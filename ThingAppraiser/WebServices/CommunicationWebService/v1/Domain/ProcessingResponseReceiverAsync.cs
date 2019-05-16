using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using ThingAppraiser.Data.Models;

namespace ThingAppraiser.CommunicationWebService.v1.Domain
{
    public class ProcessingResponseReceiverAsync : IProcessingResponseReceiverAsync
    {
        private readonly ServiceSettings _settings;


        public ProcessingResponseReceiverAsync(IOptions<ServiceSettings> settingsOptions)
        {
            _settings = settingsOptions.Value.ThrowIfNull(nameof(settingsOptions));
        }

        #region IProcessingResponseReceiverAsync Implementation

        public async Task<ProcessingResponse> ReceiveProcessingResponseAsync(RequestData requestData)
        {
            using (var client = new HttpClient
                   { BaseAddress = new Uri(_settings.ProcessingServiceBaseAddress) })
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json")
                );

                var responseMessage = await client.PostAsJsonAsync(
                    _settings.ProcessingServiceApiUrl, requestData
                );
                if (responseMessage.IsSuccessStatusCode)
                {
                    var result = await responseMessage.Content.ReadAsAsync<ProcessingResponse>();
                    return result;
                }
            }

            throw new Exception("Data processing request failed.");
        }

        #endregion
    }
}
