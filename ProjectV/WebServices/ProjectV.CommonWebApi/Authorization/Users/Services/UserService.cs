using System;
using System.Threading.Tasks;
using Acolyte.Assertions;
using Microsoft.Extensions.Options;
using ProjectV.CommonWebApi.Authorization.Passwords;
using ProjectV.CommonWebApi.Authorization.Tokens.Services;
using ProjectV.Configuration.Options;
using ProjectV.DataAccessLayer.Services.Tokens;
using ProjectV.DataAccessLayer.Services.Users;
using ProjectV.Models.Authorization;
using ProjectV.Models.Authorization.Tokens;
using ProjectV.Models.Users;
using ProjectV.Models.WebServices.Requests;
using ProjectV.Models.WebServices.Responses;

namespace ProjectV.CommonWebApi.Authorization.Users.Services
{
    public sealed class UserService : IUserService
    {
        private readonly UserServiceOptions _userServiceOptions;
        private readonly ITokenService _tokenService;
        private readonly IPasswordManager _passwordManager;
        private readonly IUserInfoService _userInfoService;
        private readonly IRefreshTokenInfoService _refreshTokenInfoService;

        public UserService(
            IOptions<UserServiceOptions> userServiceOptions,
            ITokenService tokenService,
            IPasswordManager passwordManager,
            IUserInfoService userInfoService,
            IRefreshTokenInfoService refreshTokenInfoService)
        {
            _userServiceOptions = userServiceOptions.Value.ThrowIfNull(nameof(userServiceOptions));
            _tokenService = tokenService.ThrowIfNull(nameof(tokenService));
            _passwordManager = passwordManager.ThrowIfNull(nameof(passwordManager));
            _userInfoService = userInfoService.ThrowIfNull(nameof(userInfoService));
            _refreshTokenInfoService = refreshTokenInfoService.ThrowIfNull(nameof(refreshTokenInfoService));

            // Create system user if needed.
            _ = FindOrCreateSystemUserAsync();
        }

        #region IUserService Implementation

        public async Task<SignupResponse> SignupAsync(SignupRequest signupRequest)
        {
            if (!_userServiceOptions.AllowSignup)
            {
                return new SignupResponse
                {
                    Success = false,
                    ErrorMessage = "Sign up option is currently disabled",
                    ErrorCode = "S02"
                };
            }

            UserInfo? existingUser = await _userInfoService.FindByUserNameAsync(
                UserName.Wrap(signupRequest.UserName)
            );

            if (existingUser is not null)
            {
                return new SignupResponse
                {
                    Success = false,
                    ErrorMessage = "User already exists with the same user name",
                    ErrorCode = "S03"
                };
            }

            var password = Password.Wrap(signupRequest.Password);
            var confirmPassword = Password.Wrap(signupRequest.ConfirmPassword);

            if (password != confirmPassword)
            {
                return new SignupResponse
                {
                    Success = false,
                    ErrorMessage = "Password and confirm password do not match",
                    ErrorCode = "S04"
                };
            }

            if (!_passwordManager.EnsurePasswordIsStrong(password))
            {
                return new SignupResponse
                {
                    Success = false,
                    ErrorMessage = "Password is weak",
                    ErrorCode = "S05"
                };
            }

            var userName = UserName.Wrap(signupRequest.UserName);
            UserInfo user = CreateUser(userName, password, signupRequest.Timestamp);

            int addCounter = await _userInfoService.AddAsync(user);

            if (addCounter >= 0)
            {
                return new SignupResponse
                {
                    Success = true,
                    UserId = user.Id.Value,
                    UserName = user.UserName.Value
                };
            }

            return new SignupResponse
            {
                Success = false,
                ErrorMessage = "Unable to save the user",
                ErrorCode = "S06"
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
                    ErrorMessage = "User name not found",
                    ErrorCode = "L02"
                };
            }

            var loginPassword = Password.Wrap(loginRequest.Password);
            var loginPasswordHash = _passwordManager.HashUsingPbkdf2(
                loginPassword, Convert.FromBase64String(user.PasswordSalt)
            );

            if (user.Password != loginPasswordHash)
            {
                return new TokenResponse
                {
                    Success = false,
                    ErrorMessage = "Invalid password",
                    ErrorCode = "L03"
                };
            }

            TokensHolder? token = await Task.Run(() => _tokenService.GenerateTokensAsync(user.Id));
            if (token is null)
            {
                return new TokenResponse
                {
                    Success = false,
                    ErrorMessage = "Failed to generate tokens",
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
                ErrorMessage = "Unable to logout user",
                ErrorCode = "L05"
            };
        }

        #endregion

        public async Task<UserInfo?> FindOrCreateSystemUserAsync()
        {
            if (!_userServiceOptions.ShouldCreateSystemUser)
            {
                return null;
            }

            if (string.IsNullOrWhiteSpace(_userServiceOptions.SystemUserName) ||
                string.IsNullOrWhiteSpace(_userServiceOptions.SystemUserPassword))
            {
                throw new InvalidOperationException(
                    "Failed to create system user: no data specified."
                );
            }

            var systemUserName = UserName.Wrap(_userServiceOptions.SystemUserName);
            var systemUserPassword = Password.Wrap(_userServiceOptions.SystemUserPassword);

            UserInfo? systemUser = await _userInfoService.FindByUserNameAsync(systemUserName);
            if (systemUser is not null)
            {
                return systemUser;
            }

            UserInfo createdUser = CreateUser(systemUserName, systemUserPassword, DateTime.UtcNow);
            int addCounter = await _userInfoService.AddAsync(createdUser);

            if (addCounter >= 0)
            {
                return createdUser;
            }

            throw new InvalidOperationException("Failed to save system user.");
        }

        private UserInfo CreateUser(UserName userName, Password password, DateTime timestamp)
        {
            var salt = _passwordManager.GetSecureSalt();
            var passwordHash = _passwordManager.HashUsingPbkdf2(password, salt);

            return new UserInfo(
                id: UserId.Create(),
                userName: userName,
                password: passwordHash,
                passwordSalt: Convert.ToBase64String(salt),
                timestampUtc: timestamp.ToUniversalTime(),
                active: true, // You can save is false and send confirmation email to the user, then once the user confirms the email you can make it true.
                refreshToken: null
            );
        }
    }
}
