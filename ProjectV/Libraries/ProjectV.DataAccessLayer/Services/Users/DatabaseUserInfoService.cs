using System.Threading.Tasks;
using Acolyte.Assertions;
using AutoMapper;
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

        private readonly IMapper _mapper;


        public DatabaseUserInfoService(
            ProjectVDbContext context,
            IMapper mapper)
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
                var userDbModel = _mapper.Map<UserDbInfo>(userInfo);

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

            return _mapper.Map<UserInfo>(userDbModel);
        }

        public async Task<UserInfo?> FindByUserNameAsync(UserName userName)
        {
            UserDbInfo? userDbModel = await _context.ExecuteIfCanUseDb(
                () => _context.GetUserDbSet(),
                dbSet => dbSet.SingleOrDefaultAsync(user => user.WrappedUserName == userName)
            );

            return _mapper.Map<UserInfo>(userDbModel);
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
                var userDbModel = _mapper.Map<UserDbInfo>(userInfo);

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
                var jobDbModel = _mapper.Map<UserDbInfo>(jobInfo);

                dbSet.Remove(jobDbModel);
                return await _context.SaveChangesAsync();
            }
        }

        #endregion
    }
}
