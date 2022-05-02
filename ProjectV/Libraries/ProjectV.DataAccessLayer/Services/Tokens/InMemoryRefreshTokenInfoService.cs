using System;
using System.Threading.Tasks;
using Acolyte.Assertions;
using ProjectV.DataAccessLayer.Services.Basic;
using ProjectV.DataAccessLayer.Services.Users;
using ProjectV.Models.Authorization.Tokens;
using ProjectV.Models.Users;

namespace ProjectV.DataAccessLayer.Services.Tokens
{
    public sealed class InMemoryRefreshTokenInfoService :
        InMemoryDataService<RefreshTokenId, RefreshTokenInfo>, IRefreshTokenInfoService, IDisposable
    {
        private readonly InMemoryUserInfoService _userInfoService;


        public InMemoryRefreshTokenInfoService(
            IUserInfoService userInfoService)
        {
            userInfoService.ThrowIfNull(nameof(userInfoService));

            // We accept interface to allow DI inject object.
            if (userInfoService is not InMemoryUserInfoService inMemoryUserInfoService)
            {
                throw new ArgumentException("In-memory IUserInfoService expected.", nameof(userInfoService));
            }

            _userInfoService = inMemoryUserInfoService;

            _userInfoService.OnAddAsync += AddTokenForUserIfNeededAsync;
            _userInfoService.OnUpdateAsync += UpdateTokenForUserIfNeededAsync;
            _userInfoService.OnDeleteAsync += DeleteTokenForUserIfNeededAsync;

            OnAddAsync += UpdateUserForTokenIfNeededAsync;
        }

        #region IDisposable Members

        private bool _disposed;

        public void Dispose()
        {
            if (_disposed) return;

            _userInfoService.OnAddAsync -= AddTokenForUserIfNeededAsync;
            _userInfoService.OnUpdateAsync -= UpdateTokenForUserIfNeededAsync;
            _userInfoService.OnDeleteAsync -= DeleteTokenForUserIfNeededAsync;

            OnAddAsync -= UpdateUserForTokenIfNeededAsync;

            _disposed = true;
        }

        #endregion

        #region IRefreshTokenInfoService Implementation

        public async Task<RefreshTokenInfo?> FindByUserIdAsync(UserId userId)
        {
            UserInfo? userInfo = await _userInfoService.FindByIdAsync(userId);
            if (userInfo?.RefreshToken is null)
            {
                return null;
            }

            // Get actual value from storage.
            if (_storage.TryGetValue(userInfo.RefreshToken.Id, out RefreshTokenInfo? info))
            {
                return info;
            }

            return null;
        }

        #endregion

        private async Task AddTokenForUserIfNeededAsync(UserInfo userInfo)
        {
            if (userInfo.RefreshToken is not null)
            {
                await AddAsync(userInfo.RefreshToken);
            }
        }

        private async Task UpdateTokenForUserIfNeededAsync(UserInfo userInfo)
        {
            if (userInfo.RefreshToken is not null)
            {
                await UpdateAsync(userInfo.RefreshToken);
            }
        }

        private async Task DeleteTokenForUserIfNeededAsync(UserInfo userInfo)
        {
            if (userInfo.RefreshToken is not null)
            {
                await DeleteAsync(userInfo.RefreshToken.Id);
            }
        }

        private async Task UpdateUserForTokenIfNeededAsync(RefreshTokenInfo refreshToken)
        {
            UserInfo? userInfo = await _userInfoService.FindByIdAsync(refreshToken.UserId);
            if (userInfo is null)
            {
                throw new InvalidOperationException(
                    $"Trying to add token for non-existed or deleted user '{refreshToken.UserId}'."
                );
            }

            if (userInfo.RefreshToken is null)
            {
                var userInfoWithToken = new UserInfo(
                    id: userInfo.Id,
                    userName: userInfo.UserName,
                    password: userInfo.Password,
                    passwordSalt: userInfo.PasswordSalt,
                    timestamp: userInfo.Timestamp,
                    active: userInfo.Active,
                    refreshToken: refreshToken
                );
                await _userInfoService.UpdateAsync(userInfoWithToken);
            }
        }
    }
}
