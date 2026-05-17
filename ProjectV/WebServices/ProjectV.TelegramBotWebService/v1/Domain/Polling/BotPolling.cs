using System.Threading;
using System.Threading.Tasks;
using Acolyte.Assertions;
using Microsoft.Extensions.Options;
using ProjectV.Configuration;
using ProjectV.Logging;
using ProjectV.TelegramBotWebService.Options;
using ProjectV.TelegramBotWebService.v1.Domain.Bot;
using ProjectV.TelegramBotWebService.v1.Domain.Polling.Handlers;
using Telegram.Bot;
using Telegram.Bot.Polling;

namespace ProjectV.TelegramBotWebService.v1.Domain.Polling
{
    public sealed class BotPolling : IBotPolling
    {
        private static readonly ILogger _logger = LoggerFactory.CreateLoggerFor<BotPolling>();

        private readonly TelegramBotWebServiceOptions _options;

        private readonly IBotService _botService;

        private readonly IBotPollingUpdateHandler _updateHandler;

        private bool DropPendingUpdatesOnDelete => _options.Bot.Webhook.DropPendingUpdatesOnDelete;


        public BotPolling(
            IOptions<TelegramBotWebServiceOptions> options,
            IBotService botService,
            IBotPollingUpdateHandler updateHandler)
        {
            _options = options.GetCheckedValue();
            _botService = botService.ThrowIfNull(nameof(botService));
            _updateHandler = updateHandler.ThrowIfNull(nameof(updateHandler));
        }

        #region IBotPolling Implementation

        public async Task StartReceivingUpdatesAsync(CancellationToken cancellationToken)
        {
            _logger.Info("Need to delete webhook at first to start receiving updates via polling.");

            await _botService.DeleteWebhookAsync(DropPendingUpdatesOnDelete, cancellationToken);

            _logger.Info("Starting receiving updates from Telegram API via polling.");

            var pollingOptions = _options.Bot.Polling;
            var receiverOptions = new ReceiverOptions
            {
                Offset = pollingOptions.Offset,
                AllowedUpdates = pollingOptions.AllowedUpdates,
                Limit = pollingOptions.Limit,
                DropPendingUpdates = pollingOptions.DropPendingUpdates,
            };

            await _botService.BotClient.ReceiveAsync(
                updateHandler: _updateHandler,
                receiverOptions: receiverOptions,
                cancellationToken: cancellationToken
            );

            _logger.Info("Receiving updates from Telegram API via polling has been finished.");
        }

        #endregion
    }
}
