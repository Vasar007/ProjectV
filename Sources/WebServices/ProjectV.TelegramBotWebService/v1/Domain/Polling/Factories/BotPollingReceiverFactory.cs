using Acolyte.Assertions;
using ProjectV.Logging;
using ProjectV.TelegramBotWebService.Options;
using Telegram.Bot.Polling;

namespace ProjectV.TelegramBotWebService.v1.Domain.Polling.Factories
{
    public sealed class BotPollingReceiverFactory : IBotPollingReceiverFactory
    {
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor<BotPollingReceiverFactory>();


        public BotPollingReceiverFactory()
        {
        }

        #region IBotPollingReceiverFactory Implementation

        public ReceiverOptions Create(BotPollingOptions options)
        {
            options.ThrowIfNull(nameof(options));

            _logger.Info($"Creating receiver options for polling mode: {options.ProcessingMode}.");

            return new ReceiverOptions
            {
                Offset = options.Offset,
                AllowedUpdates = options.AllowedUpdates,
                Limit = options.Limit,
                DropPendingUpdates = options.DropPendingUpdates,
            };
        }

        #endregion
    }
}
