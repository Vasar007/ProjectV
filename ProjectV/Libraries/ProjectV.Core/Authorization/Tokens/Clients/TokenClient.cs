using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Acolyte.Assertions;
using Acolyte.Common;
using Microsoft.Extensions.Options;
using ProjectV.Configuration.Options;
using ProjectV.Core.Net.Http;
using ProjectV.Logging;
using ProjectV.Models.WebServices.Requests;
using ProjectV.Models.WebServices.Responses;

namespace ProjectV.Core.Authorization.Tokens.Clients
{
    public sealed class TokenClient : ITokenClient
    {
        private static readonly ILogger _logger = LoggerFactory.CreateLoggerFor<TokenClient>();

        private readonly HttpClient _client;
        private readonly string _loginApiUrl;
        private readonly bool _shouldDisposeHttpClient;
        private readonly bool _continueOnCapturedContext;


        // We added this ctor to reuse HttpClient from other client wrappers.
        public TokenClient(
           HttpClient client,
           string loginApiUrl,
           bool shouldDisposeHttpClient,
           bool continueOnCapturedContext)
        {
            _client = client.ThrowIfNull(nameof(client));
            _loginApiUrl = loginApiUrl.ThrowIfNull(nameof(loginApiUrl));
            _shouldDisposeHttpClient = shouldDisposeHttpClient;
            _continueOnCapturedContext = continueOnCapturedContext;
        }

        public TokenClient(
           IHttpClientFactory httpClientFactory,
           ProjectVServiceOptions serviceOptions)
            : this(
                CreateClient(httpClientFactory, serviceOptions),
                serviceOptions.RestApi.CommunicationServiceLoginApiUrl,
                serviceOptions.HttpClient.ShouldDisposeHttpClient,
                continueOnCapturedContext: false
            )
        {
        }

        public TokenClient(
            IHttpClientFactory httpClientFactory,
            IOptions<ProjectVServiceOptions> serivceSettings)
            : this(
                httpClientFactory,
                serivceSettings.ThrowIfNull(nameof(serivceSettings)).Value
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

            _client.DisposeClient(_shouldDisposeHttpClient);

            _disposed = true;
        }

        #endregion

        #region ITokenClient Implementation

        public async Task<Result<TokenResponse, ErrorResponse>> LoginAsync(LoginRequest login)
        {
            login.ThrowIfNull(nameof(login));

            var request = new HttpRequestMessage(HttpMethod.Post, _loginApiUrl)
                .AsJson(login);

            return await _client.SendAndReadAsync<TokenResponse>(
                    request, _logger, _continueOnCapturedContext, CancellationToken.None
                )
                .ConfigureAwait(_continueOnCapturedContext);
        }

        #endregion

        private static HttpClient CreateClient(IHttpClientFactory httpClientFactory,
            ProjectVServiceOptions serviceOptions)
        {
            httpClientFactory.ThrowIfNull(nameof(httpClientFactory));
            serviceOptions.ThrowIfNull(nameof(serviceOptions));

            string baseAddress = serviceOptions.RestApi.CommunicationServiceBaseAddress;
            return httpClientFactory.CreateClientWithOptions(baseAddress, serviceOptions.HttpClient);
        }
    }
}
