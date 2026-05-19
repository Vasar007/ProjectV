using System.Threading.Tasks;
using Acolyte.Assertions;
using Acolyte.Exceptions;
using Microsoft.EntityFrameworkCore;
using ProjectV.DataAccessLayer.Services.Basic;
using ProjectV.DataAccessLayer.Services.Users.Models;
using ProjectV.Models.Users;

namespace ProjectV.DataAccessLayer.Services.Users
{
    public sealed class DatabaseUserInfoService :
        DataServiceBase<UserId, UserInfo>, IUserInfoService
    {
        private readonly ProjectVDbContext _context;

        private readonly DataAccessLayerMapper _mapper;


        public DatabaseUserInfoService(
            ProjectVDbContext context,
            DataAccessLayerMapper mapper)
        {
            _context = context.ThrowIfNull(nameof(context));
            _mapper = mapper.ThrowIfNull(nameof(mapper));
        }

        #region IUserInfoService Implementation

        public async Task<int> AddAsync(UserInfo userInfo)
        {
            userInfo.ThrowIfNull(nameof(userInfo));

            return await _context.ExecuteIfCanUseDb(
                 () => _context.GetUserDbSet(),
                 dbSet => AddUserAsync(dbSet)
             );

            async ValueTask<int> AddUserAsync(DbSet<UserDbInfo> dbSet)
            {
                var userDbModel = _mapper.MapToUserDbInfo(userInfo);

                await dbSet.AddAsync(userDbModel);
                return await _context.SaveChangesAsync();
            }
        }

        public override async Task<UserInfo?> FindByIdAsync(UserId userId)
        {
            UserDbInfo? userDbModel = await _context.ExecuteIfCanUseDb(
                () => _context.GetUserDbSet(),
                dbSet => dbSet.FindAsync(userId.Value)
            );

            return userDbModel is null ? null : _mapper.MapToUserInfo(userDbModel);
        }

        public async Task<UserInfo?> FindByUserNameAsync(UserName userName)
        {
            // EF Core cannot translate `user.WrappedUserName == userName` —
            // WrappedUserName is a computed property (UserName.Wrap(UserName))
            // and the underlying scalar `UserName` field is internal. Compare
            // against the raw string column directly; the SUT input is the
            // domain `UserName` so we read its .Value (Plan 02-09 Task 1
            // Rule 1 fix).
            string rawUserName = userName.Value;
            UserDbInfo? userDbModel = await _context.ExecuteIfCanUseDb(
                () => _context.GetUserDbSet(),
                dbSet => dbSet.SingleOrDefaultAsync(user => user.UserName == rawUserName)
            );

            return userDbModel is null ? null : _mapper.MapToUserInfo(userDbModel);
        }

        public async Task<UserInfo> GetByUserNameAsync(UserName userName)
        {
            var info = await FindByUserNameAsync(userName);
            if (info is null)
            {
                throw new NotFoundException($"Failed to found info by name '{userName.Value}'.");
            }

            return info;
        }

        public async Task<int> UpdateAsync(UserInfo userInfo)
        {
            userInfo.ThrowIfNull(nameof(userInfo));

            return await _context.ExecuteIfCanUseDb(
                 () => _context.GetUserDbSet(),
                 dbSet => UpdateUserAsync(dbSet)
             );

            async ValueTask<int> UpdateUserAsync(DbSet<UserDbInfo> dbSet)
            {
                var userDbModel = _mapper.MapToUserDbInfo(userInfo);

                dbSet.Update(userDbModel);
                return await _context.SaveChangesAsync();
            }
        }

        public async Task<int> DeleteAsync(UserId userId)
        {
            return await _context.ExecuteIfCanUseDb(
                 () => _context.GetUserDbSet(),
                 dbSet => DeleteUserAsync(dbSet)
             );

            async ValueTask<int> DeleteUserAsync(DbSet<UserDbInfo> dbSet)
            {
                UserInfo? jobInfo = await FindByIdAsync(userId);
                if (jobInfo is null)
                {
                    return 0;
                }

                var jobDbModel = _mapper.MapToUserDbInfo(jobInfo);

                dbSet.Remove(jobDbModel);
                return await _context.SaveChangesAsync();
            }
        }

        #endregion
    }
}
