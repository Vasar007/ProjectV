using System;
using System.Threading.Tasks;
using ProjectV.Models.Users;

namespace ProjectV.CommonWebApi.Authorization.Tokens.Generators
{
    public interface ITokenGenerator
    {
        public TimeSpan AccessTokenExpirationTimeout { get; }
        public TimeSpan RefreshTokenExpirationTimeout { get; }

        Task<string> GenerateAccessTokenAsync(UserId userId);
        Task<string> GenerateRefreshTokenAsync();
    }
}
