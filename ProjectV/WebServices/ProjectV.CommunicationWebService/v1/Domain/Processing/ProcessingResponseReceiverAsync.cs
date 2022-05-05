using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Acolyte.Assertions;
using Microsoft.Extensions.Options;
using ProjectV.CommunicationWebService.Config;
using ProjectV.Logging;
using ProjectV.Models.WebServices.Responses;

namespace ProjectV.CommunicationWebService.v1.Domain.Processing
{
    public sealed class ProcessingResponseReceiverAsync : IProcessingResponseReceiverAsync,
        IDisposable
    {
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor<ProcessingResponseReceiverAsync>();

        private readonly CommunicationWebServiceSettings _settings;

        private readonly HttpClient _client;


        public ProcessingResponseReceiverAsync(IOptions<CommunicationWebServiceSettings> settingsOptions)
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
            StartJobDataResponce jobData)
        {
            _logger.Info("Sending data request and trying to receive response.");

            using (HttpResponseMessage responseMessage = await _client.PostAsJsonAsync(
                    _settings.ProcessingServiceApiUrl, jobData
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
