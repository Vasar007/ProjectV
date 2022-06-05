using System;
using Acolyte.Assertions;
using ProjectV.Logging;
using ProjectV.TelegramBotWebService.Options;
using ProjectV.TelegramBotWebService.v1.Domain.Bot;
using Telegram.Bot.Extensions.Polling;

namespace ProjectV.TelegramBotWebService.v1.Domain.Polling.Factories
{
    public sealed class BotPollingReceiverFactory : IBotPollingReceiverFactory
    {
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor<BotPollingReceiverFactory>();

        private readonly IBotService _botService;


        public BotPollingReceiverFactory(
            IBotService botService)
        {
            _botService = botService.ThrowIfNull(nameof(botService));
        }

        #region IBotPollingProcessorFactory Implementation

        public IUpdateReceiver Create(BotPollingOptions options)
        {
            options.ThrowIfNull(nameof(options));

            _logger.Info($"Creating bot polling receiver for mode: {options.ProcessingMode}.");

            var receiverOptions = CreateReceiverOptions(options);

            return options.ProcessingMode switch
            {
                _ when options.IsMode(BotPollingProcessingMode.LoopReceiving) => new DefaultUpdateReceiver(_botService.BotClient, receiverOptions),

                _ when options.IsMode(BotPollingProcessingMode.AsyncQueuedReceiving) => new DefaultUpdateReceiver(_botService.BotClient, receiverOptions),

                _ when options.IsMode(BotPollingProcessingMode.BlockingReceiving) => new DefaultUpdateReceiver(_botService.BotClient, receiverOptions),

                _ => throw new ArgumentOutOfRangeException(
                    nameof(options),
                    options.ProcessingMode,
                    $"Unexpected processing mode: '{options.ProcessingMode}'."
                )
            };
        }

        #endregion

        private static ReceiverOptions CreateReceiverOptions(BotPollingOptions options)
        {
            return new ReceiverOptions
            {
                Offset = options.Offset,
                AllowedUpdates = options.AllowedUpdates,
                Limit = options.Limit,
                ThrowPendingUpdates = options.ThrowPendingUpdates
            };
        }
    }
}
