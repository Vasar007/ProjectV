using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Acolyte.Assertions;
using ProjectV.Configuration.Options;
using ProjectV.Logging;
using ProjectV.Models.WebService.Requests;
using ProjectV.Models.WebService.Responses;

namespace ProjectV.DesktopApp.Models.DataSuppliers
{
    internal sealed class ServiceProxy : IDisposable
    {
        private static readonly ILogger _logger = LoggerFactory.CreateLoggerFor<ServiceProxy>();

        private readonly ProjectVServiceOptions _serviceOptions;

        private string BaseAddress => _serviceOptions.CommunicationServiceBaseAddress;

        private string ApiUrl => _serviceOptions.CommunicationServiceApiUrl;

        private readonly HttpClient _client;


        public ServiceProxy(
            ProjectVServiceOptions serviceOptions)
        {
            _serviceOptions = serviceOptions.ThrowIfNull(nameof(serviceOptions));

            _logger.Info($"ProjectV service url: {BaseAddress}");

            _client = new HttpClient { BaseAddress = new Uri(BaseAddress) };
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json")
            );
        }

        public async Task<ProcessingResponse?> SendPostRequest(StartJobParamsRequest requestParams)
        {
            requestParams.ThrowIfNull(nameof(requestParams));

            _logger.Info($"Service method '{nameof(SendPostRequest)}' is called.");

            using HttpResponseMessage response = await _client.PostAsJsonAsync(
                ApiUrl, requestParams
            );

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsAsync<ProcessingResponse>();
                return result;
            }

            return null;
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
    }
}
