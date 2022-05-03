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

        private readonly HttpClient _client;

        private string BaseAddress => _serviceOptions.CommunicationServiceBaseAddress;

        private string ApiUrl => _serviceOptions.CommunicationServiceApiUrl;


        public ServiceProxy(
            ProjectVServiceOptions serviceOptions)
        {
            _serviceOptions = serviceOptions.ThrowIfNull(nameof(serviceOptions));

            _logger.Info($"ProjectV service url: {BaseAddress}");

            _client = CreateHttpClient(serviceOptions);
        }

        public async Task<ProcessingResponse?> SendPostRequest(StartJobParamsRequest requestParams)
        {
            requestParams.ThrowIfNull(nameof(requestParams));

            _logger.Info($"Sending POST request to '{ApiUrl}'.");

            using var response = await _client.PostAsJsonAsync(ApiUrl, requestParams);

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
