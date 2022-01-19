using System.Net.Http;
using Acolyte.Assertions;
using Microsoft.Extensions.Options;
using MihaZupan;
using Telegram.Bot;

namespace ProjectV.TelegramBotWebService.v1.Domain
{
    public sealed class BotService : IBotService
    {
        private readonly BotConfiguration _config;

        #region IBotService Implementation

        public TelegramBotClient Client { get; }

        #endregion


        public BotService(IOptions<BotConfiguration> config)
        {
            _config = config.Value.ThrowIfNull(nameof(config));

            // Use proxy if configured in appsettings.*.json
            Client = !_config.UseProxy
                ? new TelegramBotClient(_config.BotToken)
                : new TelegramBotClient(
                    _config.BotToken,
                    new HttpClient(new HttpClientHandler { Proxy = new HttpToSocks5Proxy(_config.Socks5Host, _config.Socks5Port), UseProxy = true })
                );
        }

    }
}
