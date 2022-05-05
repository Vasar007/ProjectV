using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Acolyte.Assertions;
using Microsoft.Extensions.Options;
using ProjectV.CommunicationWebService.Config;
using ProjectV.Logging;
using ProjectV.Models.Configuration;
using ProjectV.Models.WebServices.Requests;
using ProjectV.Models.WebServices.Responses;

namespace ProjectV.CommunicationWebService.v1.Domain.Configuration
{
    public sealed class ConfigurationReceiverAsync : IConfigurationReceiverAsync, IDisposable
    {
        private static readonly ILogger _logger =
           LoggerFactory.CreateLoggerFor<ConfigurationReceiverAsync>();

        private readonly CommunicationWebServiceSettings _settings;

        private readonly HttpClient _client;


        public ConfigurationReceiverAsync(
            IOptions<CommunicationWebServiceSettings> settingsOptions)
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

        public async Task<StartJobDataResponce> ReceiveConfigForRequestAsync(
            StartJobParamsRequest jobParams)
        {
            _logger.Info("Sending config request and trying to receive response.");

            using (HttpResponseMessage responseConfigMessage = await _client.PostAsJsonAsync(
                       _settings.ConfigurationServiceApiUrl, jobParams.Requirements
                  )
            )
            {
                if (responseConfigMessage.IsSuccessStatusCode)
                {
                    _logger.Info("Received successful config response.");

                    var config =
                        await responseConfigMessage.Content.ReadAsAsync<ConfigurationXml>();

                    var requestData = new StartJobDataResponce
                    {
                        ThingNames = jobParams.ThingNames,
                        ConfigurationXml = config
                    };
                    return requestData;
                }
            }

            _logger.Info("Received bad config response.");

            throw new Exception("Config request failed.");
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
