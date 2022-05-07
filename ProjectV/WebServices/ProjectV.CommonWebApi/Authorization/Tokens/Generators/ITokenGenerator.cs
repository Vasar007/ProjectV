using System;
using System.Threading.Tasks;
using ProjectV.Models.Authorization.Tokens;
using ProjectV.Models.Users;

namespace ProjectV.CommonWebApi.Authorization.Tokens.Generators
{
    public interface ITokenGenerator
    {
        Task<AccessTokenData> GenerateAccessTokenAsync(UserId userId, DateTime utcDateTime);
        Task<RefreshTokenData> GenerateRefreshTokenAsync(DateTime utcDateTime);
    }
}
