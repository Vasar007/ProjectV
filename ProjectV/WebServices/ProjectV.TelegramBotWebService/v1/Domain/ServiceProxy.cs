using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Acolyte.Assertions;
using Microsoft.Extensions.Options;
using ProjectV.Logging;
using ProjectV.Models.WebService;

namespace ProjectV.TelegramBotWebService.v1.Domain
{
    public sealed class ServiceProxy : IServiceProxy, IDisposable
    {
        private static readonly ILogger _logger = LoggerFactory.CreateLoggerFor<ServiceProxy>();

        private readonly ServiceSettings _settings;

        private readonly HttpClient _client;

        private bool _disposed;


        public ServiceProxy(IOptions<ServiceSettings> settings)
        {
            _settings = settings.Value.ThrowIfNull(nameof(settings));

            _logger.Info("ProjectV service url: " +
                         $"{_settings.ProjectVServiceBaseAddress}");

            _client = new HttpClient
            {
                BaseAddress = new Uri(_settings.ProjectVServiceBaseAddress)
            };
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json")
            );
        }

        #region IServiceProxy Implementation

        public async Task<ProcessingResponse?> SendPostRequest(RequestParams requestParams)
        {
            _logger.Info("Service method 'PostInitialRequest' is called.");

            using HttpResponseMessage response = await _client.PostAsJsonAsync(
                _settings.ProjectVServiceApiUrl, requestParams
            );

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsAsync<ProcessingResponse>();
                return result;
            }

            return null;
        }

        #endregion

        #region IDisposable Implementation

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            _client.Dispose();
        }

        #endregion
    }
}