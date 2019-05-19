using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace ThingAppraiser.TelegramBotWebService.v1.Domain
{
    public class ServiceSetupAsync : IServiceSetupAsync, IDisposable
    {
        private readonly ServiceSettings _settings;

        private readonly BotConfiguration _config;

        private readonly HttpClient _client;

        private bool _disposedValue;


        public ServiceSetupAsync(IOptions<ServiceSettings> settings,
            IOptions<BotConfiguration> config)
        {
            _settings = settings.Value.ThrowIfNull(nameof(settings));
            _config = config.Value.ThrowIfNull(nameof(config));

            _client = new HttpClient
            {
                BaseAddress = new Uri(_settings.TelegramApiUrl)
            };
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json")
            );
        }

        #region IServiceSetupAsync Implementation

        public async Task SetWebHook()
        {
            string requestUrl = string.Format(
                _settings.SetWebhookRequestUrlTemplate,
                _config.BotToken,                
                _settings.NgrokUrl,
                _settings.ServiceApiUrl
            );
            using (HttpResponseMessage response = await _client.GetAsync(requestUrl))
            {
                response.EnsureSuccessStatusCode();

                string result = await response.Content.ReadAsStringAsync();

                // Success example: {"ok": true, "result": true, "description": "Webhook was set"}
                // Failed example: {"ok": false, "error_code": 401, "description": "Unauthorized"}
                JToken jsonData = JToken.Parse(result);
                if (!jsonData.Value<bool>("ok"))
                {
                    int errorCode = jsonData.Value<int>("error_code");
                    string description = jsonData.Value<string>("description");
                    throw new ArgumentException(
                        "Invalid base or request URL.\n" +
                        $"Error code: {errorCode}, description: {description}"
                    );
                }
            }
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
