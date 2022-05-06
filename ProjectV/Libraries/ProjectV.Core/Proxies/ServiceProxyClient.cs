using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Acolyte.Assertions;
using Acolyte.Common;
using Microsoft.Extensions.Options;
using Polly;
using ProjectV.Configuration.Options;
using ProjectV.Core.Logging;
using ProjectV.Core.Net.Http;
using ProjectV.Core.Net.Polly;
using ProjectV.Logging;
using ProjectV.Models.WebServices.Requests;
using ProjectV.Models.WebServices.Responses;

namespace ProjectV.Core.Proxies
{
    public sealed class ServiceProxyClient : IServiceProxyClient
    {
        private static readonly ILogger _logger = LoggerFactory.CreateLoggerFor<ServiceProxyClient>();

        private const string AccessTokenKey = "AccessToken";
        private const string RefreshTokenKey = "RefreshToken";

        private readonly ProjectVServiceOptions _serviceOptions;
        private readonly UserServiceOptions _userServiceOptions;

        private readonly HttpClient _client;

        private string RequestApiUrl => _serviceOptions.CommunicationServiceRequestApiUrl;
        private string LoginApiUrl => _serviceOptions.CommunicationServiceLoginApiUrl;


        public ServiceProxyClient(
           IHttpClientFactory httpClientFactory,
           ProjectVServiceOptions serviceOptions,
           UserServiceOptions userServiceOptions)
        {
            _serviceOptions = serviceOptions.ThrowIfNull(nameof(serviceOptions));
            _userServiceOptions = userServiceOptions.ThrowIfNull(nameof(userServiceOptions));
            httpClientFactory.ThrowIfNull(nameof(httpClientFactory));

            _client = httpClientFactory.CreateClientWithOptions(serviceOptions);
        }

        public ServiceProxyClient(
            IHttpClientFactory httpClientFactory,
            IOptions<ProjectVServiceOptions> serivceSettings,
            IOptions<UserServiceOptions> userServiceSettings)
            : this(
                httpClientFactory,
                serivceSettings.ThrowIfNull(nameof(serivceSettings)).Value,
                userServiceSettings.ThrowIfNull(nameof(userServiceSettings)).Value
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

            _client.DisposeClient(_serviceOptions);

            _disposed = true;
        }

        #endregion

        #region IServiceProxyClient Implementation

        public async Task<Result<TokenResponse, ErrorResponse>> LoginAsync(LoginRequest login)
        {
            login.ThrowIfNull(nameof(login));

            var request = new HttpRequestMessage(HttpMethod.Post, LoginApiUrl)
                .AsJson(login);

            return await _client.SendAndReadAsync<TokenResponse>(request, _logger);
        }

        public async Task<Result<ProcessingResponse, ErrorResponse>> StartJobAsync(
            StartJobParamsRequest jobParams)
        {
            jobParams.ThrowIfNull(nameof(jobParams));

            // Using policy to catch unauthorized error and handle it.
            // This trick requires to recreate HttpRequestMessage, so we cannot do it with
            // predefined global policies for HttpClient.
            var refreshAuthenticationPolicy = PolicyCreator.HandleUnauthorizedAsync(
                _serviceOptions, RefreshAuthorizationAsync
            );

            var response = await refreshAuthenticationPolicy.ExecuteAsync(
                context => StartJobInternalAsync(jobParams, context),
                new Dictionary<string, object?>
                {
                    { AccessTokenKey, _serviceOptions.AccessToken },
                    { RefreshTokenKey, _serviceOptions.RefreshToken }
                }
            );

            return await response.ReadContentAsAsync<ProcessingResponse>(_logger);
        }

        #endregion

        private async Task<Result<TokenResponse, ErrorResponse>> LoginToRefreshAuthorizationAsync()
        {
            // TODO: add option to login for user and use user's access token.
            if (string.IsNullOrWhiteSpace(_userServiceOptions.SystemUserName) ||
                string.IsNullOrWhiteSpace(_userServiceOptions.SystemUserPassword))
            {
                _logger.Warn("No system user specified. Cannot refresh authorization.");
                var response = new ErrorResponse();
                return Result.Error(response);
            }

            _logger.Warn("Trying to use system user to refresh authorization.");
            var login = new LoginRequest
            {
                UserName = _userServiceOptions.SystemUserName,
                Password = _userServiceOptions.SystemUserPassword
            };

            return await LoginAsync(login);
        }

        private async Task RefreshAuthorizationAsync(DelegateResult<HttpResponseMessage> outcome,
           TimeSpan sleepDuration, int retryCount, Context context)
        {
            _logger.LogRetryingInfo(outcome, sleepDuration, retryCount);

            var result = await LoginToRefreshAuthorizationAsync();

            if (result.IsSuccess && result.Ok is not null)
            {
                var tokenResponse = result.Ok;
                context[AccessTokenKey] = tokenResponse.AccessToken;
                context[RefreshTokenKey] = tokenResponse.RefreshToken;
            }
        }

        private async Task<HttpResponseMessage> StartJobInternalAsync(
            StartJobParamsRequest jobParams, Context context)
        {
            jobParams.ThrowIfNull(nameof(jobParams));

            var request = new HttpRequestMessage(HttpMethod.Post, RequestApiUrl)
                .AsJson(jobParams);

            if (context.TryGetValue(AccessTokenKey, out object? value) &&
                value is string accessToken &&
                !string.IsNullOrWhiteSpace(accessToken))
            {
                var authentication = new AuthenticationHeaderValue("Bearer", accessToken);
                request.Headers.Authorization = authentication;
            }

            return await _client.SendAsync(request);
        }
    }
}
