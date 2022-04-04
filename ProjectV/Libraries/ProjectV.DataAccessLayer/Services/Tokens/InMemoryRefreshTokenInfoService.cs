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
            InMemoryUserInfoService userInfoService)
        {
            _userInfoService = userInfoService.ThrowIfNull(nameof(userInfoService));

            _userInfoService.OnAddAsync += AddIfNeededAsync;
            _userInfoService.OnUpdateAsync += UpdateIfNeededAsync;
            _userInfoService.OnDeleteAsync += DeleteIfNeededAsync;
        }

        #region IDisposable Members

        private bool _disposed;

        public void Dispose()
        {
            if (_disposed) return;

            _userInfoService.OnAddAsync -= AddIfNeededAsync;
            _userInfoService.OnUpdateAsync -= UpdateIfNeededAsync;
            _userInfoService.OnDeleteAsync -= DeleteIfNeededAsync;

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

        private async Task AddIfNeededAsync(UserInfo userInfo)
        {
            if (userInfo.RefreshToken is not null)
            {
                await AddAsync(userInfo.RefreshToken);
            }
        }

        private async Task UpdateIfNeededAsync(UserInfo userInfo)
        {
            if (userInfo.RefreshToken is not null)
            {
                await UpdateAsync(userInfo.RefreshToken);
            }
        }

        private async Task DeleteIfNeededAsync(UserInfo userInfo)
        {
            if (userInfo.RefreshToken is not null)
            {
                await DeleteAsync(userInfo.RefreshToken.Id);
            }
        }
    }
}
