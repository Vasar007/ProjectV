using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Acolyte.Assertions;
using Microsoft.Extensions.Options;
using ProjectV.Logging;
using ProjectV.Models.WebService.Requests;
using ProjectV.Models.WebService.Responses;
using ProjectV.TelegramBotWebService.Config;

namespace ProjectV.TelegramBotWebService.v1.Domain.Proxy
{
    public sealed class ServiceProxy : IServiceProxy, IDisposable
    {
        private static readonly ILogger _logger = LoggerFactory.CreateLoggerFor<ServiceProxy>();

        private readonly TelegramBotWebServiceSettings _settings;

        private readonly HttpClient _client;

        private string BaseAddress => _settings.ProjectVServiceBaseAddress;

        private string ApiUrl => _settings.ProjectVServiceApiUrl;


        public ServiceProxy(
            IOptions<TelegramBotWebServiceSettings> settings)
        {
            _settings = settings.Value.ThrowIfNull(nameof(settings));

            _logger.Info($"ProjectV service url: {BaseAddress}");

            _client = CreateHttpClient(_settings);
        }

        #region IServiceProxy Implementation

        public async Task<ProcessingResponse?> SendPostRequest(StartJobParamsRequest jobParams)
        {
            _logger.Info($"Sending POST request to '{ApiUrl}'.");

            using var response = await _client.PostAsJsonAsync(ApiUrl, jobParams);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsAsync<ProcessingResponse>();
                return result;
            }

            return null;
        }

        #endregion

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

        private static HttpClient CreateHttpClient(TelegramBotWebServiceSettings settings)
        {
            string baseAddress = settings.ProjectVServiceBaseAddress;

            var client = new HttpClient
            {
                BaseAddress = new Uri(baseAddress)
            };

            try
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // TODO: add option to login for user and use user's access token.
                if (!string.IsNullOrWhiteSpace(settings.AccessToken))
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {settings.AccessToken}");
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
