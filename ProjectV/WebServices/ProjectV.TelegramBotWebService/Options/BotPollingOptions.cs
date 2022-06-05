using Acolyte.Assertions;
using ProjectV.Configuration;
using Telegram.Bot.Types.Enums;

namespace ProjectV.TelegramBotWebService.Options
{
    public sealed class BotPollingOptions : IOptions
    {
        public BotPollingProcessingMode ProcessingMode { get; set; } =
            BotPollingProcessingMode.Default;

        public int? Offset { get; set; } = null;

        public UpdateType[]? AllowedUpdates { get; set; } = null;

        public int? Limit { get; set; } = null;

        public bool ThrowPendingUpdates { get; set; } = false;


        public BotPollingOptions()
        {
        }

        public BotPollingProcessingMode GetProcessingModeForDefault()
        {
            return BotPollingProcessingMode.AsyncQueuedReceiving;
        }

        public bool IsMode(BotPollingProcessingMode expectedMode)
        {
            expectedMode.ThrowIfEnumValueIsUndefined(nameof(expectedMode));

            if (ProcessingMode == BotPollingProcessingMode.Default)
            {
                return GetProcessingModeForDefault() == expectedMode;
            }

            return ProcessingMode == expectedMode;
        }
    }
}
