using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Acolyte.Assertions;
using Acolyte.Common;
using Microsoft.Extensions.Options;
using Polly;
using ProjectV.Configuration;
using ProjectV.Configuration.Options;
using ProjectV.Core.Authorization.Tokens.Caches;
using ProjectV.Core.Authorization.Tokens.Clients;
using ProjectV.Core.Logging;
using ProjectV.Core.Net.Http;
using ProjectV.Core.Net.Polly;
using ProjectV.Logging;
using ProjectV.Models.Authorization.Tokens;
using ProjectV.Models.WebServices.Requests;
using ProjectV.Models.WebServices.Responses;

namespace ProjectV.Core.Services.Clients
{
    public sealed class CommunicationServiceClient : ICommunicationServiceClient
    {
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor<CommunicationServiceClient>();

        private const string AccessTokenKey = "access_token";
        private const string RefreshTokenKey = "refresh_token";

        private readonly ProjectVServiceOptions _serviceOptions;
        private readonly UserServiceOptions _userServiceOptions;

        private readonly HttpClient _client;
        private readonly bool _continueOnCapturedContext;
        private readonly ITokenClient _tokenClient;
        private readonly ITokenCache _tokenCache;

        private string BaseAddress => _serviceOptions.RestApi.CommunicationServiceBaseAddress;
        private string RequestApiUrl => _serviceOptions.RestApi.CommunicationServiceRequestApiUrl;
        private string LoginApiUrl => _serviceOptions.RestApi.CommunicationServiceLoginApiUrl;
        private HttpClientOptions HcOptions => _serviceOptions.HttpClient;


        public CommunicationServiceClient(
           IHttpClientFactory httpClientFactory,
           ProjectVServiceOptions serviceOptions,
           UserServiceOptions userServiceOptions)
        {
            httpClientFactory.ThrowIfNull(nameof(httpClientFactory));
            _serviceOptions = serviceOptions.ThrowIfNull(nameof(serviceOptions));
            _userServiceOptions = userServiceOptions.ThrowIfNull(nameof(userServiceOptions));

            try
            {
                _client = httpClientFactory.CreateClientWithOptions(BaseAddress, HcOptions);
                _continueOnCapturedContext = false;

                _tokenClient = new TokenClient(
                    _client,
                    LoginApiUrl,
                    HcOptions.ShouldDisposeHttpClient,
                    _continueOnCapturedContext
                );

                _tokenCache = new TokenCache(_tokenClient, _continueOnCapturedContext);
            }
            catch (Exception)
            {
                _client.DisposeClient(HcOptions);
                _tokenClient.DisposeSafe();
                throw;
            }
        }

        public CommunicationServiceClient(
            IHttpClientFactory httpClientFactory,
            IOptions<ProjectVServiceOptions> serivceSettings,
            IOptions<UserServiceOptions> userServiceSettings)
            : this(
                httpClientFactory,
                serivceSettings.GetCheckedValue(),
                userServiceSettings.GetCheckedValue()
            )
        {
        }

        #region IDisposable Implementation

        /// <summary>
        /// Boolean flag used to show that object has already been disposed.
        /// </summary>
        private bool _disposed;

        public void Dispose()
        {
            if (_disposed) return;

            _client.DisposeClient(HcOptions);
            _tokenClient.Dispose();

            _disposed = true;
        }

        #endregion

        #region ICommunicationProxyClient Implementation

        public async Task<Result<TokenResponse, ErrorResponse>> LoginAsync(LoginRequest login)
        {
            login.ThrowIfNull(nameof(login));

            return await _tokenClient.LoginAsync(login)
                .ConfigureAwait(_continueOnCapturedContext);
        }

        public async Task<Result<ProcessingResponse, ErrorResponse>> StartJobAsync(
            StartJobParamsRequest jobParams)
        {
            jobParams.ThrowIfNull(nameof(jobParams));

            // Using policy to catch unauthorized error and handle it.
            // This trick requires to recreate HttpRequestMessage, so we cannot do it with
            // predefined global policies for HttpClient.
            var refreshAuthenticationPolicy = PolicyCreator.HandleUnauthorizedAsync(
                HcOptions, RefreshAuthorizationAsync
            );

            // Try to get authorization tokens from cache.
            var result = await TryGetTokensFromCacheAsync()
               .ConfigureAwait(_continueOnCapturedContext);

            // If we cannot get tokens -> it is an error.
            if (!result.IsSuccess)
            {
                return Result.Error(result.Error!);
            }

            // If everything is ok, try to send job request.
            var tokenResponse = result.Ok!;

            // Using refresh policy to retry on unauthorized error.
            var response = await refreshAuthenticationPolicy.ExecuteAsync(
                (context, token) => StartJobInternalAsync(jobParams, context),
                new Dictionary<string, object?>
                {
                    { AccessTokenKey, tokenResponse.AccessToken },
                    { RefreshTokenKey, tokenResponse.RefreshToken }
                },
                CancellationToken.None,
                _continueOnCapturedContext
            ).ConfigureAwait(_continueOnCapturedContext);

            // Read result.
            return await response.ReadContentAsAsync<ProcessingResponse>(
                    _logger, _continueOnCapturedContext, CancellationToken.None
                )
                .ConfigureAwait(_continueOnCapturedContext);
        }

        #endregion

        private async Task<Result<TokenResponse, ErrorResponse>> TryGetTokensFromCacheAsync()
        {
            // Try to get authorization tokens.
            var result = await GetTokensToRefreshAuthorization(forceRefresh: false)
               .ConfigureAwait(_continueOnCapturedContext);

            // Process result and append additional data if needed.
            if (!result.IsSuccess)
            {
                return result;
            }
            else if (result.Ok is null)
            {
                string message = "Cannot get authorization tokens: tokens are invalid.";
                _logger.Warn(message);

                var error = new ErrorResponse
                {
                    Success = false,
                    ErrorCode = "T02",
                    ErrorMessage = message
                };
                return Result.Error(error);
            }

            return result;
        }

        private async Task<Result<TokenResponse, ErrorResponse>> GetTokensToRefreshAuthorization(
            bool forceRefresh)
        {
            // TODO: add option to login for user and use user's access token.
            if (!_userServiceOptions.CanUseSystemUserToAuthenticate)
            {
                string message = "Cannot get authorization tokens: system user cannot be used.";
                _logger.Warn(message);

                var response = new ErrorResponse
                {
                    Success = false,
                    ErrorCode = "T01",
                    ErrorMessage = message
                };
                return Result.Error(response);
            }

            if (string.IsNullOrWhiteSpace(_userServiceOptions.SystemUserName) ||
                string.IsNullOrWhiteSpace(_userServiceOptions.SystemUserPassword))
            {
                string message = "Cannot get authorization tokens: no system user specified.";
                _logger.Warn(message);

                var response = new ErrorResponse
                {
                    Success = false,
                    ErrorCode = "T02",
                    ErrorMessage = message
                };
                return Result.Error(response);
            }

            _logger.Info("Creating system user login to get authorization tokens.");
            var login = new LoginRequest
            {
                UserName = _userServiceOptions.SystemUserName,
                Password = _userServiceOptions.SystemUserPassword
            };

            return await _tokenCache.GetTokensAsync(login, forceRefresh)
                .ConfigureAwait(_continueOnCapturedContext);
        }

        private async Task RefreshAuthorizationAsync(DelegateResult<HttpResponseMessage> outcome,
           TimeSpan sleepDuration, int retryCount, Context context)
        {
            _logger.LogRetryingInfo(outcome, sleepDuration, retryCount);

            var result = await GetTokensToRefreshAuthorization(forceRefresh: true)
                .ConfigureAwait(_continueOnCapturedContext);

            if (result.IsSuccess && result.Ok is not null)
            {
                var tokenResponse = result.Ok;

                // Update tokens in the context.
                context[AccessTokenKey] = tokenResponse.AccessToken.Token;
                context[RefreshTokenKey] = tokenResponse.RefreshToken.Token;
            }
        }

        private async Task<HttpResponseMessage> StartJobInternalAsync(
            StartJobParamsRequest jobParams, Context context)
        {
            jobParams.ThrowIfNull(nameof(jobParams));

            var request = new HttpRequestMessage(HttpMethod.Post, RequestApiUrl)
                .AsJson(jobParams);

            if (context.TryGetValue(AccessTokenKey, out object? value) &&
                value is AccessTokenData accessToken &&
                !string.IsNullOrWhiteSpace(accessToken.Token))
            {
                var authentication = new AuthenticationHeaderValue("Bearer", accessToken.Token);
                request.Headers.Authorization = authentication;
            }

            return await _client.SendAsync(request)
                .ConfigureAwait(_continueOnCapturedContext);
        }
    }
}
