using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Acolyte.Assertions;
using Acolyte.Common;
using Microsoft.Extensions.Options;
using ProjectV.Configuration.Options;
using ProjectV.Logging;
using ProjectV.Models.WebServices.Requests;
using ProjectV.Models.WebServices.Responses;

namespace ProjectV.Core.Proxies
{
    public sealed class ServiceProxy : IServiceProxy
    {
        private static readonly ILogger _logger = LoggerFactory.CreateLoggerFor<ServiceProxy>();

        private readonly ProjectVServiceOptions _serviceOptions;

        private readonly HttpClient _client;

        private string BaseAddress => _serviceOptions.CommunicationServiceBaseAddress;

        private string RequestApiUrl => _serviceOptions.CommunicationServiceRequestApiUrl;


        public ServiceProxy(
           ProjectVServiceOptions serviceOptions)
        {
            _serviceOptions = serviceOptions.ThrowIfNull(nameof(serviceOptions));

            _logger.Info($"ProjectV service URL: {BaseAddress}");

            _client = CreateHttpClient(_serviceOptions);
        }

        public ServiceProxy(
            IOptions<ProjectVServiceOptions> settings)
            : this(settings.ThrowIfNull(nameof(settings)).Value)
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

            _client.Dispose();

            _disposed = true;
        }

        #endregion

        #region IServiceProxy Implementation

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
            return Result.Error(error);
        }

        #endregion

        private static HttpClient CreateHttpClient(ProjectVServiceOptions serviceOptions)
        {
            string baseAddress = serviceOptions.CommunicationServiceBaseAddress;

            var client = new HttpClient
            {
                BaseAddress = new Uri(baseAddress)
            };

            try
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // TODO: add option to login for user and use user's access token.
                if (!string.IsNullOrWhiteSpace(serviceOptions.AccessToken))
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {serviceOptions.AccessToken}");
                }

                return client;
            }
            catch (Exception)
            {
                client.Dispose();
                throw;
            }
        }
    }
}
