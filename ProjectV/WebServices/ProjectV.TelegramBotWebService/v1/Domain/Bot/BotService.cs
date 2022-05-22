using System.Net.Http;
using Acolyte.Assertions;
using Microsoft.Extensions.Options;
using MihaZupan;
using ProjectV.TelegramBotWebService.Options;
using Telegram.Bot;

namespace ProjectV.TelegramBotWebService.v1.Domain.Bot
{
    public sealed class BotService : IBotService
    {
        private readonly BotOptions _config;

        #region IBotService Implementation

        public ITelegramBotClient Client { get; }

        #endregion


        public BotService(
           IHttpClientFactory httpClientFactory,
            IOptions<BotOptions> config)
        {
            httpClientFactory.ThrowIfNull(nameof(httpClientFactory));
            _config = config.Value.ThrowIfNull(nameof(config));

            // Use proxy if configured in appsettings.*.json
            Client = CreateClient(_config);
        }

        private static TelegramBotClient CreateClient(BotOptions config)
        {
            if (!config.UseProxy)
            {
                return new TelegramBotClient(config.BotToken);
            }

            return new TelegramBotClient(
                config.BotToken,
                new HttpClient(new HttpClientHandler { Proxy = new HttpToSocks5Proxy(config.Socks5HostName, config.Socks5Port), UseProxy = true })
            );
        }
    }
}
