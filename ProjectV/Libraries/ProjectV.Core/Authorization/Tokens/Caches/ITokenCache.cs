using System.Threading;
using System.Threading.Tasks;
using Acolyte.Common;
using ProjectV.Models.WebServices.Requests;
using ProjectV.Models.WebServices.Responses;

namespace ProjectV.Core.Authorization.Tokens.Caches
{
    public interface ITokenCache
    {
        Task<Result<TokenResponse, ErrorResponse>> GetTokensAsync(LoginRequest login,
            bool forceRefresh, CancellationToken cancellationToken);
    }
}
