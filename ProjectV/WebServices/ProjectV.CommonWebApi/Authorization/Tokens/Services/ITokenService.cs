using System.Threading.Tasks;
using ProjectV.Models.Users;

namespace ProjectV.CommonWebApi.Authorization.Tokens.Services
{
    public interface ITokenService
    {
        Task<(string, string)> GenerateTokensAsync(UserId userId);
        //Task<ValidateRefreshTokenResponse> ValidateRefreshTokenAsync(RefreshTokenRequest refreshTokenRequest);
        //Task<bool> RemoveRefreshTokenAsync(User user);

        // https://codingsonata.com/apply-jwt-access-tokens-and-refresh-tokens-in-asp-net-core-web-api-6/
    }
}
