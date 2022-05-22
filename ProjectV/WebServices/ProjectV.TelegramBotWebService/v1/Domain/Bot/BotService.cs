using System;
using System.Net.Http;
using Acolyte.Assertions;
using Microsoft.Extensions.Options;
using ProjectV.Configuration.Options;
using ProjectV.Core.Net.Http;
using ProjectV.TelegramBotWebService.Options;
using Telegram.Bot;

namespace ProjectV.TelegramBotWebService.v1.Domain.Bot
{
    public sealed class BotService : IBotService
    {
        private readonly ProjectVServiceOptions _serviceOptions;
        private readonly TelegramBotWebServiceOptions _botServiceOptions;

        private readonly HttpClient _httpClient;
        private readonly bool _continueOnCapturedContext;

        #region IBotService Implementation

        public ITelegramBotClient Client { get; }

        #endregion

        private HttpClientOptions HcOptions => _serviceOptions.HttpClient;
        private string BotToken => _botServiceOptions.Bot.Token;


        public BotService(
            IHttpClientFactory httpClientFactory,
            IOptions<ProjectVServiceOptions> serviceOptions,
            IOptions<TelegramBotWebServiceOptions> botServiceOptions)
        {
            httpClientFactory.ThrowIfNull(nameof(httpClientFactory));
            _serviceOptions = serviceOptions.Value.ThrowIfNull(nameof(serviceOptions));
            _botServiceOptions = botServiceOptions.Value.ThrowIfNull(nameof(botServiceOptions));

            try
            {
                _httpClient = httpClientFactory.CreateClientWithOptions(HcOptions);
                _continueOnCapturedContext = false;

                Client = new TelegramBotClient(BotToken, _httpClient);
            }
            catch (Exception)
            {
                _httpClient.DisposeClient(HcOptions);
                throw;
            }
        }

        #region IDisposable Implementation

        /// <summary>
        /// Boolean flag used to show that object has already been disposed.
        /// </summary>
        private bool _disposed;

        public void Dispose()
        {
            if (_disposed) return;

            _httpClient.DisposeClient(HcOptions);

            _disposed = true;
        }

        #endregion
    }
}
