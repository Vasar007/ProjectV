using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using ThingAppraiser.Data.Configuration;
using ThingAppraiser.Data.Models;

namespace ThingAppraiser.CommunicationWebService.v1.Domain
{
    public class ConfigurationReceiverAsync : IConfigurationReceiverAsync
    {
        private readonly ServiceSettings _settings;


        public ConfigurationReceiverAsync(IOptions<ServiceSettings> settingsOptions)
        {
            _settings = settingsOptions.Value.ThrowIfNull(nameof(settingsOptions));
        }

        #region IConfigurationReceiverAsync Implementation

        public async Task<RequestData> ReceiveConfigForRequestAsync(RequestParams requestParams)
        {
            using (var client = new HttpClient
                   { BaseAddress = new Uri(_settings.ConfigurationServiceBaseAddress) })
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json")
                );

                var responseConfigMessage = await client.PostAsJsonAsync(
                    _settings.ConfigurationServiceApiUrl, requestParams.ConfigRequirements
                );
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
    }
}
