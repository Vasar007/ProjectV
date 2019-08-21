using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using ThingAppraiser.Models.Configuration;
using ThingAppraiser.Models.WebService;

namespace ThingAppraiser.CommunicationWebService.v1.Domain
{
    public sealed class ConfigurationReceiverAsync : IConfigurationReceiverAsync, IDisposable
    {
        private readonly ServiceSettings _settings;

        private readonly HttpClient _client;

        private bool _disposedValue;


        public ConfigurationReceiverAsync(IOptions<ServiceSettings> settingsOptions)
        {
            _settings = settingsOptions.Value.ThrowIfNull(nameof(settingsOptions));

            _client = new HttpClient
            {
                BaseAddress = new Uri(_settings.ConfigurationServiceBaseAddress)
            };
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json")
            );
        }

        #region IConfigurationReceiverAsync Implementation

        public async Task<RequestData> ReceiveConfigForRequestAsync(RequestParams requestParams)
        {
            using (HttpResponseMessage responseConfigMessage = await _client.PostAsJsonAsync(
                       _settings.ConfigurationServiceApiUrl, requestParams.Requirements
                   ))
            {
                if (responseConfigMessage.IsSuccessStatusCode)
                {
                    var config =
                        await responseConfigMessage.Content.ReadAsAsync<ConfigurationXml>();

                    var requestData = new RequestData
                    {
                        ThingNames = requestParams.ThingNames,
                        ConfigurationXml = config
                    };
                    return requestData;
                }
            }

            throw new Exception("Config request failed.");
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
