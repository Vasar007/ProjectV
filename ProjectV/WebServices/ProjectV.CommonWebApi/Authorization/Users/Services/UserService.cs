using System;
using System.Threading.Tasks;
using Acolyte.Assertions;
using ProjectV.CommonWebApi.Authorization.Passwords;
using ProjectV.CommonWebApi.Authorization.Tokens.Services;
using ProjectV.DataAccessLayer.Services.Tokens;
using ProjectV.DataAccessLayer.Services.Users;
using ProjectV.Models.Authorization.Tokens;
using ProjectV.Models.Users;
using ProjectV.Models.WebServices.Requests;
using ProjectV.Models.WebServices.Responses;

namespace ProjectV.CommonWebApi.Authorization.Users.Services
{
    public sealed class UserService : IUserService
    {
        private readonly ITokenService _tokenService;
        private readonly IPasswordManager _passwordManager;
        private readonly IUserInfoService _userInfoService;
        private readonly IRefreshTokenInfoService _refreshTokenInfoService;

        public UserService(
            ITokenService tokenService,
            IPasswordManager passwordManager,
            IUserInfoService userInfoService,
            IRefreshTokenInfoService refreshTokenInfoService)
        {
            _tokenService = tokenService.ThrowIfNull(nameof(tokenService));
            _passwordManager = passwordManager.ThrowIfNull(nameof(passwordManager));
            _userInfoService = userInfoService.ThrowIfNull(nameof(userInfoService));
            _refreshTokenInfoService = refreshTokenInfoService.ThrowIfNull(nameof(refreshTokenInfoService));
        }

        #region IUserService Implementation

        public async Task<SignupResponse> SignupAsync(SignupRequest signupRequest)
        {
            UserInfo? existingUser = await _userInfoService.FindByUserNameAsync(
                UserName.Wrap(signupRequest.UserName)
            );

            if (existingUser is not null)
            {
                return new SignupResponse
                {
                    Success = false,
                    Error = "User already exists with the same email",
                    ErrorCode = "S02"
                };
            }

            if (StringComparer.Ordinal.Equals(signupRequest.Password, signupRequest.ConfirmPassword))
            {
                return new SignupResponse
                {
                    Success = false,
                    Error = "Password and confirm password do not match",
                    ErrorCode = "S03"
                };
            }

            if (!_passwordManager.EnsurePasswordIsStrong(signupRequest.Password))
            {
                return new SignupResponse
                {
                    Success = false,
                    Error = "Password is weak",
                    ErrorCode = "S04"
                };
            }

            byte[] salt = _passwordManager.GetSecureSalt();
            string passwordHash = _passwordManager.HashUsingPbkdf2(signupRequest.Password, salt);

            var user = new UserInfo(
                id: UserId.Create(),
                userName: UserName.Wrap(signupRequest.UserName),
                password: passwordHash,
                passwordSalt: Convert.ToBase64String(salt),
                ts: signupRequest.Ts,
                active: true, // You can save is false and send confirmation email to the user, then once the user confirms the email you can make it true
                refreshToken: null
            );

            int addCounter = await _userInfoService.AddAsync(user);

            if (addCounter >= 0)
            {
                return new SignupResponse
                {
                    Success = true,
                    UserName = user.UserName.Value
                };
            }

            return new SignupResponse
            {
                Success = false,
                Error = "Unable to save the user",
                ErrorCode = "S05"
            };
        }

        public async Task<TokenResponse> LoginAsync(LoginRequest loginRequest)
        {
            UserInfo? user = await _userInfoService.FindByUserNameAsync(
                UserName.Wrap(loginRequest.UserName)
            );

            if (user is null)
            {
                return new TokenResponse
                {
                    Success = false,
                    Error = "Email not found",
                    ErrorCode = "L02"
                };
            }
            string passwordHash = _passwordManager.HashUsingPbkdf2(
                loginRequest.Password, Convert.FromBase64String(user.PasswordSalt)
            );

            if (user.Password != passwordHash)
            {
                return new TokenResponse
                {
                    Success = false,
                    Error = "Invalid Password",
                    ErrorCode = "L03"
                };
            }

            TokensHolder? token = await Task.Run(() => _tokenService.GenerateTokensAsync(user.Id));
            if (token is null)
            {
                return new TokenResponse
                {
                    Success = false,
                    Error = "Failed to generate tokens",
                    ErrorCode = "L04"
                };
            }

            return new TokenResponse
            {
                Success = true,
                AccessToken = token.AccessToken,
                RefreshToken = token.RefreshToken,
            };
        }

        public async Task<LogoutResponse> LogoutAsync(UserId userId)
        {
            RefreshTokenInfo? refreshToken = await _refreshTokenInfoService.FindByUserIdAsync(userId);

            if (refreshToken is null)
            {
                return new LogoutResponse
                {
                    Success = true
                };
            }

            int deleteCounter = await _refreshTokenInfoService.DeleteAsync(refreshToken.Id);

            if (deleteCounter >= 0)
            {
                return new LogoutResponse
                {
                    Success = true
                };
            }

            return new LogoutResponse
            {
                Success = false,
                Error = "Unable to logout user",
                ErrorCode = "L05"
            };
        }

        #endregion
    }
}