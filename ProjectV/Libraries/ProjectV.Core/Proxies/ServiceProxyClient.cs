using System.Net.Http;
using System.Threading.Tasks;
using Acolyte.Assertions;
using Acolyte.Common;
using Microsoft.Extensions.Options;
using ProjectV.Configuration.Options;
using ProjectV.Core.Net.Http;
using ProjectV.Logging;
using ProjectV.Models.WebServices.Requests;
using ProjectV.Models.WebServices.Responses;

namespace ProjectV.Core.Proxies
{
    public sealed class ServiceProxyClient : IServiceProxyClient
    {
        // TODO 2: process error response and on 401 error try to authorize and retry.

        private static readonly ILogger _logger = LoggerFactory.CreateLoggerFor<ServiceProxyClient>();

        private readonly ProjectVServiceOptions _serviceOptions;

        private readonly HttpClient _client;

        private string RequestApiUrl => _serviceOptions.CommunicationServiceRequestApiUrl;


        public ServiceProxyClient(
           IHttpClientFactory httpClientFactory,
           ProjectVServiceOptions serviceOptions)
        {
            _serviceOptions = serviceOptions.ThrowIfNull(nameof(serviceOptions));
            httpClientFactory.ThrowIfNull(nameof(httpClientFactory));

            _client = httpClientFactory.CreateClientWithOptions(serviceOptions);
        }

        public ServiceProxyClient(
            IHttpClientFactory httpClientFactory,
            IOptions<ProjectVServiceOptions> settings)
            : this(httpClientFactory, settings.ThrowIfNull(nameof(settings)).Value)
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

        public async Task<Result<ProcessingResponse, ErrorResponse>> SendRequest(
            StartJobParamsRequest jobParams)
        {
            jobParams.ThrowIfNull(nameof(jobParams));

            _logger.Info($"Sending POST request to '{RequestApiUrl}'.");

            using var response = await _client.PostAsJsonAsync(RequestApiUrl, jobParams);

            if (response.IsSuccessStatusCode)
            {
                _logger.Info($"Got a success status code {response.StatusCode} from '{RequestApiUrl}'.");
                var result = await response.Content.ReadAsAsync<ProcessingResponse>();
                return Result.Ok(result);
            }

            _logger.Error($"Got an error status code from '{RequestApiUrl}': {response.ReasonPhrase} (code: {response.StatusCode}).");

            // Response does not have content for 401 error, e.g. calling method with "Authorize"
            // attribute but request does not contain "Authorization" header.
            var error = await response.Content.ReadAsAsync<ErrorResponse>();
            if (error is null)
            {
                // In case response does not have any content, create error from common properties.
                error = new ErrorResponse
                {
                    Success = false,
                    ErrorCode = ((int)response.StatusCode).ToString(),
                    ErrorMessage = response.ReasonPhrase
                };
            }

            // TODO: process error response and on 401 error try to authorize and retry.
            return Result.Error(error);
        }

        #endregion
    }
}
