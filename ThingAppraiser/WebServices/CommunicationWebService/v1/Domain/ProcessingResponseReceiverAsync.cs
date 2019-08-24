using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using ThingAppraiser.Logging;
using ThingAppraiser.Models.WebService;

namespace ThingAppraiser.CommunicationWebService.v1.Domain
{
    public sealed class ProcessingResponseReceiverAsync : IProcessingResponseReceiverAsync,
        IDisposable
    {
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor<ProcessingResponseReceiverAsync>();

        private readonly ServiceSettings _settings;

        private readonly HttpClient _client;

        private bool _disposed;


        public ProcessingResponseReceiverAsync(IOptions<ServiceSettings> settingsOptions)
        {
            _settings = settingsOptions.Value.ThrowIfNull(nameof(settingsOptions));

            _client = new HttpClient
            {
                BaseAddress = new Uri(_settings.ProcessingServiceBaseAddress)
            };
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json")
            );
        }

        #region IProcessingResponseReceiverAsync Implementation

        public async Task<ProcessingResponse> ReceiveProcessingResponseAsync(
            RequestData requestData)
        {
            _logger.Info("Sending data request and trying to receive response.");

            using (HttpResponseMessage responseMessage = await _client.PostAsJsonAsync(
                    _settings.ProcessingServiceApiUrl, requestData
                  )
            )
            {
                if (responseMessage.IsSuccessStatusCode)
                {
                    _logger.Info("Received successful data response.");

                    var result = await responseMessage.Content.ReadAsAsync<ProcessingResponse>();
                    return result;
                }
            }

            _logger.Info("Received bad data response.");

            throw new Exception("Data processing request failed.");
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
