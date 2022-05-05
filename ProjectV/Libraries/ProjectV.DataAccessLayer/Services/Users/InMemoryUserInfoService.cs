using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Acolyte.Exceptions;
using ProjectV.DataAccessLayer.Services.Basic;
using ProjectV.Models.Users;

namespace ProjectV.DataAccessLayer.Services.Users
{
    public sealed class InMemoryUserInfoService :
        InMemoryDataService<UserId, UserInfo>, IUserInfoService
    {
        public InMemoryUserInfoService()
        {
        }

        #region IUserInfoService Implementation

        public Task<UserInfo?> FindByUserNameAsync(UserName userName)
        {
            KeyValuePair<UserId, UserInfo> kvp = _storage
                .SingleOrDefault(pair => pair.Value.UserName == userName);

            return Task.FromResult<UserInfo?>(kvp.Value);
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

        #endregion
    }
}
