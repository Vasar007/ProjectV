using System;
using System.Threading.Tasks;
using Acolyte.Assertions;
using Acolyte.Common;
using ProjectV.Core.Authorization.Tokens.Clients;
using ProjectV.Models.WebServices.Requests;
using ProjectV.Models.WebServices.Responses;

namespace ProjectV.Core.Authorization.Tokens.Caches
{
    public sealed class TokenCache : ITokenCache
    {
        private readonly ITokenClient _tokenClient;
        private readonly bool _continueOnCapturedContext;

        private Result<TokenResponse, ErrorResponse>? _cachedResponse;


        public TokenCache(
            ITokenClient tokenClient,
            bool continueOnCapturedContext)
        {
            _tokenClient = tokenClient.ThrowIfNull(nameof(tokenClient));
            _continueOnCapturedContext = continueOnCapturedContext;
        }

        public async Task<Result<TokenResponse, ErrorResponse>> GetTokensAsync(LoginRequest login,
            bool forceRefresh)
        {
            login.ThrowIfNull(nameof(login));

            if (!forceRefresh && IsTokenValid())
            {
                // Additional null check.
                if (_cachedResponse is not null)
                {
                    return _cachedResponse.Value;
                }
            }

            _cachedResponse = await RequestToken(login)
                .ConfigureAwait(_continueOnCapturedContext);

            return _cachedResponse.Value;
        }

        private bool IsTokenValid()
        {
            if (_cachedResponse is null)
            {
                return false;
            }

            if (!_cachedResponse.Value.IsSuccess)
            {
                return false;
            }

            if (_cachedResponse.Value.Ok is null)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(_cachedResponse.Value.Ok.AccessToken.Token))
            {
                return false;
            }
            
            // Now we check only access token.
            return _cachedResponse.Value.Ok.AccessToken.ExpiryDateUtc > DateTime.UtcNow;
        }

        private async Task<Result<TokenResponse, ErrorResponse>> RequestToken(LoginRequest login)
        {
            return await _tokenClient.LoginAsync(login)
                .ConfigureAwait(_continueOnCapturedContext);
        }
    }
}
