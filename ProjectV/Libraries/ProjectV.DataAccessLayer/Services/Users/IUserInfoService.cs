using System.Threading.Tasks;
using ProjectV.DataAccessLayer.Services.Basic;
using ProjectV.Models.Users;

namespace ProjectV.DataAccessLayer.Services.Users
{
    public interface IUserInfoService : IDataInfoServiceBase<UserId, UserInfo>
    {
        Task<UserInfo?> FindByUserNameAsync(UserName userName);
        Task<UserInfo> GetByUserNameAsync(UserName userName);
    }
}
