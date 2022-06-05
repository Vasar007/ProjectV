using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Acolyte.Assertions;
using Acolyte.Common.Disposal;
using Microsoft.Extensions.Options;
using ProjectV.Configuration;
using ProjectV.Logging;
using ProjectV.TelegramBotWebService.Options;
using ProjectV.TelegramBotWebService.v1.Domain.Bot;
using ProjectV.TelegramBotWebService.v1.Domain.Polling.Handlers;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types.InputFiles;

namespace ProjectV.TelegramBotWebService.v1.Domain.Polling
{
    public sealed class BotPolling : IBotPolling
    {
        private static readonly ILogger _logger = LoggerFactory.CreateLoggerFor<BotPolling>();

        private readonly TelegramBotWebServiceOptions _options;

        private readonly IBotService _botService;

        private readonly IBotPollingUpdateHandler _botPollingUpdateHandler;


        public BotPolling(
            IOptions<TelegramBotWebServiceOptions> options,
            IBotService botService,
            IBotPollingUpdateHandler botPollingUpdateHandler)
        {
            _options = options.GetCheckedValue();
            _botService = botService.ThrowIfNull(nameof(botService));
            _botPollingUpdateHandler = botPollingUpdateHandler.ThrowIfNull(nameof(botPollingUpdateHandler));
        }

        #region IBotPolling Implementation

        public async Task ResiveUpdatesAsync(CancellationToken cancellationToken)
        {
            _logger.Info("Starting receiving updates from Telegram API via polling.");

            var receiverOptions = CreateOptions();

            await _botService.ReceiveAsync(
                _botPollingUpdateHandler, receiverOptions, cancellationToken
            );

            _logger.Info("Receiving updates from Telegram API via polling has been finished.");
        }

        #endregion

        private ReceiverOptions CreateOptions()
        {
            return new ReceiverOptions
            {
                AllowedUpdates = default,
                Limit = default,
                Offset = default,
                ThrowPendingUpdates = default
            };
        }
    }
}
