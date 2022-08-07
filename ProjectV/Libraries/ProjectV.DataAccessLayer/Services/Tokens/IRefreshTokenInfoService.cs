using System.Threading.Tasks;
using ProjectV.DataAccessLayer.Services.Basic;
using ProjectV.Models.Authorization.Tokens;
using ProjectV.Models.Users;

namespace ProjectV.DataAccessLayer.Services.Tokens
{
    public interface IRefreshTokenInfoService :
        IDataInfoServiceBase<RefreshTokenId, RefreshTokenInfo>
    {
        Task<RefreshTokenInfo?> FindByUserIdAsync(UserId userId);
    }
}
