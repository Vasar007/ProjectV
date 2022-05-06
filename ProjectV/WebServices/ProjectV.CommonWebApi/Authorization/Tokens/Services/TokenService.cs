using System;
using System.Threading.Tasks;
using Acolyte.Assertions;
using ProjectV.CommonWebApi.Authorization.Passwords;
using ProjectV.CommonWebApi.Authorization.Tokens.Generators;
using ProjectV.DataAccessLayer.Services.Tokens;
using ProjectV.DataAccessLayer.Services.Users;
using ProjectV.Models.Authorization;
using ProjectV.Models.Authorization.Tokens;
using ProjectV.Models.Users;
using ProjectV.Models.WebServices.Requests;
using ProjectV.Models.WebServices.Responses;

namespace ProjectV.CommonWebApi.Authorization.Tokens.Services
{
    public sealed class TokenService : ITokenService
    {
        private readonly ITokenGenerator _tokenGenerator;
        private readonly IPasswordManager _passwordManager;
        private readonly IUserInfoService _userInfoService;
        private readonly IRefreshTokenInfoService _refreshTokenInfoService;


        public TokenService(
            ITokenGenerator tokenGenerator,
            IPasswordManager passwordManager,
            IUserInfoService userInfoService,
            IRefreshTokenInfoService refreshTokenInfoService)
        {
            _tokenGenerator = tokenGenerator.ThrowIfNull(nameof(tokenGenerator));
            _passwordManager = passwordManager.ThrowIfNull(nameof(passwordManager));
            _userInfoService = userInfoService.ThrowIfNull(nameof(userInfoService));
            _refreshTokenInfoService = refreshTokenInfoService.ThrowIfNull(nameof(refreshTokenInfoService));
        }

        #region ITokenService Implementation

        public async Task<TokensHolder?> GenerateTokensAsync(UserId userId)
        {
            string accessToken = await _tokenGenerator.GenerateAccessTokenAsync(userId);
            string refreshToken = await _tokenGenerator.GenerateRefreshTokenAsync();

            UserInfo? user = await _userInfoService.FindByIdAsync(userId);
            if (user is null)
            {
                return null;
            }

            var salt = _passwordManager.GetSecureSalt();
            var password = Password.Wrap(refreshToken);

            var refreshTokenHashed = _passwordManager.HashUsingPbkdf2(password, salt);

            if (user.RefreshToken is not null)
            {
                await DeleteRefreshTokenAsync(user.Id);
            }

            var refreshTokenInfo = new RefreshTokenInfo(
                id: RefreshTokenId.Create(),
                userId: userId,
                tokenHash: refreshTokenHashed,
                tokenSalt: Convert.ToBase64String(salt),
                timestampUtc: DateTime.UtcNow,
                expiryDateUtc: DateTime.UtcNow.Add(_tokenGenerator.RefreshTokenExpirationTimeout)
            );
            await _refreshTokenInfoService.AddAsync(refreshTokenInfo);

            return new TokensHolder(accessToken, refreshToken);
        }

        public async Task<bool> DeleteRefreshTokenAsync(UserId userId)
        {
            UserInfo? user = await _userInfoService.FindByIdAsync(userId);
            if (user is null)
            {
                return false;
            }

            if (user.RefreshToken is not null)
            {
                await _refreshTokenInfoService.DeleteAsync(user.RefreshToken.Id);
            }

            return false;
        }

        public async Task<ValidateRefreshTokenResponse> ValidateRefreshTokenAsync(
            RefreshTokenRequest refreshTokenRequest)
        {
            RefreshTokenInfo? refreshToken = refreshTokenRequest switch
            {
                { UserId: var userId } when userId is not null =>
                    await _refreshTokenInfoService.FindByUserIdAsync(UserId.Wrap(userId.Value)),

                { UserName: var userName } when !string.IsNullOrWhiteSpace(userName) =>
                    await FindTokenByUserNameAsync(UserName.Wrap(userName)),

                _ => null
            };

            if (refreshToken is null)
            {
                return new ValidateRefreshTokenResponse
                {
                    Success = false,
                    ErrorMessage = "Invalid request or user is already logged out",
                    ErrorCode = "R02"
                };
            }

            var password = Password.Wrap(refreshTokenRequest.RefreshToken);

            var refreshTokenToValidateHash = _passwordManager.HashUsingPbkdf2(
                password, Convert.FromBase64String(refreshToken.TokenSalt)
            );

            if (refreshToken.TokenHash != refreshTokenToValidateHash)
            {
                return new ValidateRefreshTokenResponse
                {
                    Success = false,
                    ErrorMessage = "Invalid refresh token",
                    ErrorCode = "R03"
                };
            }

            if (refreshToken.ExpiryDateUtc < DateTime.UtcNow)
            {
                return new ValidateRefreshTokenResponse
                {
                    Success = false,
                    ErrorMessage = "Refresh token has expired",
                    ErrorCode = "R04"
                };
            }

            return new ValidateRefreshTokenResponse
            {
                Success = true,
                UserId = refreshToken.UserId.Value
            };
        }

        #endregion

        private async Task<RefreshTokenInfo?> FindTokenByUserNameAsync(UserName userName)
        {
            UserInfo? user = await _userInfoService.FindByUserNameAsync(userName);
            if (user is null)
            {
                return null;
            }

            return await _refreshTokenInfoService.FindByUserIdAsync(user.Id);
        }
    }
}
