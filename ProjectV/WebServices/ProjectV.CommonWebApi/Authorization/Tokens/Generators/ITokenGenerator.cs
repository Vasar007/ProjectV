using System.Threading.Tasks;
using ProjectV.Models.Users;

namespace ProjectV.CommonWebApi.Authorization.Tokens.Generators
{
    public interface ITokenGenerator
    {
        Task<string> GenerateAccessTokenAsync(UserId userId);

        Task<string> GenerateRefreshTokenAsync();
    }
}
