using System;
using System.Linq;
using System.Threading.Tasks;
using Acolyte.Assertions;
using ProjectV.CommonWebApi.Authorization.Passwords;
using ProjectV.CommonWebApi.Authorization.Tokens.Generators;
using ProjectV.DataAccessLayer.Services.Tokens;
using ProjectV.DataAccessLayer.Services.Users;
using ProjectV.Models.Authorization;
using ProjectV.Models.Authorization.Tokens;
using ProjectV.Models.Users;
using ProjectV.Models.WebService.Requests;
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
                ts: DateTime.Now,
                expiryDate: DateTime.Now.AddDays(30)
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
            var userId = UserId.Wrap(refreshTokenRequest.UserId);

            RefreshTokenInfo? refreshToken = await _refreshTokenInfoService.FindByUserIdAsync(
                userId
            );

            var response = new ValidateRefreshTokenResponse();
            if (refreshToken is null)
            {
                response.Success = false;
                response.Error = "Invalid session or user is already logged out";
                response.ErrorCode = "R02";
                return response;
            }

            var password = Password.Wrap(refreshTokenRequest.RefreshToken);

            var refreshTokenToValidateHash = _passwordManager.HashUsingPbkdf2(
                password, Convert.FromBase64String(refreshToken.TokenSalt)
            );

            if (refreshToken.TokenHash != refreshTokenToValidateHash)
            {
                response.Success = false;
                response.Error = "Invalid refresh token";
                response.ErrorCode = "R03";
                return response;
            }

            if (refreshToken.ExpiryDate < DateTime.Now)
            {
                response.Success = false;
                response.Error = "Refresh token has expired";
                response.ErrorCode = "R04";
                return response;
            }

            response.Success = true;
            response.UserId = refreshToken.UserId;

            return response;
        }

        #endregion
    }
}
