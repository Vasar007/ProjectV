using System;
using System.Threading.Tasks;
using Acolyte.Assertions;
using Microsoft.EntityFrameworkCore;
using ProjectV.DataAccessLayer.Services.Basic;
using ProjectV.DataAccessLayer.Services.Tokens.Models;
using ProjectV.Models.Authorization.Tokens;
using ProjectV.Models.Users;

namespace ProjectV.DataAccessLayer.Services.Tokens
{
    public sealed class DatabaseRefreshTokenInfoService :
        DataServiceBase<RefreshTokenId, RefreshTokenInfo>, IRefreshTokenInfoService
    {
        private readonly ProjectVDbContext _context;

        private readonly DataAccessLayerMapper _mapper;


        public DatabaseRefreshTokenInfoService(
            ProjectVDbContext context,
            DataAccessLayerMapper mapper)
        {
            _context = context.ThrowIfNull(nameof(context));
            _mapper = mapper.ThrowIfNull(nameof(mapper));
        }

        #region IRefreshTokenInfoService Implementation

        public async Task<int> AddAsync(RefreshTokenInfo tokenInfo)
        {
            tokenInfo.ThrowIfNull(nameof(tokenInfo));

            return await _context.ExecuteIfCanUseDb(
                () => _context.GetTokenDbSet(),
                dbSet => AddTokenAsync(dbSet)
            );

            async ValueTask<int> AddTokenAsync(DbSet<RefreshTokenDbInfo> dbSet)
            {
                var tokenDbModel = _mapper.MapToRefreshTokenDbInfo(tokenInfo);

                await dbSet.AddAsync(tokenDbModel);
                return await _context.SaveChangesAsync();
            }
        }

        public override async Task<RefreshTokenInfo?> FindByIdAsync(RefreshTokenId tokenId)
        {
            RefreshTokenDbInfo? tokenDbModel = await _context.ExecuteIfCanUseDb(
                () => _context.GetTokenDbSet(),
                dbSet => dbSet.FindAsync(tokenId.Value)
            );

            return tokenDbModel is null ? null : _mapper.MapToRefreshTokenInfo(tokenDbModel);
        }

        public async Task<RefreshTokenInfo?> FindByUserIdAsync(UserId userId)
        {
            // EF Core cannot translate `token.WrappedUserId == userId` — even
            // though WrappedUserId is now a computed property, EF cannot lift
            // a static-method call (`Users.UserId.Wrap`) or a record-struct
            // comparison into SQL. Compare against the raw Guid scalar column
            // directly (Plan 02-09 Task 1 Rule 1 fix).
            Guid rawUserId = userId.Value;
            RefreshTokenDbInfo? tokenDbModel = await _context.ExecuteIfCanUseDb(
                () => _context.GetTokenDbSet(),
                dbSet => dbSet.FirstOrDefaultAsync(token => token.UserId == rawUserId)
            );

            return tokenDbModel is null ? null : _mapper.MapToRefreshTokenInfo(tokenDbModel);
        }

        public async Task<int> UpdateAsync(RefreshTokenInfo tokenInfo)
        {
            tokenInfo.ThrowIfNull(nameof(tokenInfo));

            return await _context.ExecuteIfCanUseDb(
                () => _context.GetTokenDbSet(),
                dbSet => UpdateTokenAsync(dbSet)
            );

            async ValueTask<int> UpdateTokenAsync(DbSet<RefreshTokenDbInfo> dbSet)
            {
                var tokenDbModel = _mapper.MapToRefreshTokenDbInfo(tokenInfo);

                dbSet.Update(tokenDbModel);
                return await _context.SaveChangesAsync();
            }
        }

        public async Task<int> DeleteAsync(RefreshTokenId tokenId)
        {
            return await _context.ExecuteIfCanUseDb(
                () => _context.GetTokenDbSet(),
                dbSet => DeleteTokenAsync(dbSet)
            );

            async ValueTask<int> DeleteTokenAsync(DbSet<RefreshTokenDbInfo> dbSet)
            {
                RefreshTokenInfo? tokenInfo = await FindByIdAsync(tokenId);
                if (tokenInfo is null)
                {
                    return 0;
                }

                var tokenDbModel = _mapper.MapToRefreshTokenDbInfo(tokenInfo);

                dbSet.Remove(tokenDbModel);
                return await _context.SaveChangesAsync();
            }
        }

        #endregion
    }
}
