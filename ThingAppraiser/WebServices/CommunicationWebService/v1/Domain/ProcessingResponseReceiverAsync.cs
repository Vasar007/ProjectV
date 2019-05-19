using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using ThingAppraiser.Data.Models;

namespace ThingAppraiser.CommunicationWebService.v1.Domain
{
    public class ProcessingResponseReceiverAsync : IProcessingResponseReceiverAsync, IDisposable
    {
        private readonly ServiceSettings _settings;

        private readonly HttpClient _client;

        private bool _disposedValue;


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

        public async Task<ProcessingResponse> ReceiveProcessingResponseAsync(RequestData requestData)
        {
            using (HttpResponseMessage responseMessage = await _client.PostAsJsonAsync(
                    _settings.ProcessingServiceApiUrl, requestData
                   ))
            {
                if (responseMessage.IsSuccessStatusCode)
                {
                    var result = await responseMessage.Content.ReadAsAsync<ProcessingResponse>();
                    return result;
                }
            }

            throw new Exception("Data processing request failed.");
        }

        #endregion

        #region IDisposable Implementation

        public void Dispose()
        {
            if (!_disposedValue)
            {
                _disposedValue = true;

                _client.Dispose();
            }
        }

        #endregion
    }
}
