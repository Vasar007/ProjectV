using System.Threading.Tasks;
using ProjectV.Models.Authorization.Tokens;
using ProjectV.Models.Users;
using ProjectV.Models.WebService.Requests;
using ProjectV.Models.WebServices.Responses;

namespace ProjectV.CommonWebApi.Authorization.Tokens.Services
{
    // https://codingsonata.com/apply-jwt-access-tokens-and-refresh-tokens-in-asp-net-core-web-api-6/
    public interface ITokenService
    {
        Task<TokensHolder?> GenerateTokensAsync(UserId userId);

        Task<ValidateRefreshTokenResponse> ValidateRefreshTokenAsync(RefreshTokenRequest refreshTokenRequest);

        Task<bool> DeleteRefreshTokenAsync(UserId userId);
    }
}
