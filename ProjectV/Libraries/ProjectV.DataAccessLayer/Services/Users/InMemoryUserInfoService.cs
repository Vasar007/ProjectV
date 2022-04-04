using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        #endregion
    }
}
