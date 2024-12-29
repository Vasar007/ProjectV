using Acolyte.Assertions;
using ProjectV.Configuration;
using Telegram.Bot.Types.Enums;

namespace ProjectV.TelegramBotWebService.Options
{
    public sealed class BotPollingOptions : IOptions
    {
        public BotPollingProcessingMode ProcessingMode { get; init; } =
            BotPollingProcessingMode.Default;

        public int? Offset { get; init; } = null;

        public UpdateType[]? AllowedUpdates { get; init; } = null;

        public int? Limit { get; init; } = null;

        public bool DropPendingUpdates { get; init; } = false;


        public BotPollingOptions()
        {
        }

        #region IOptions Implementation

        public void Validate()
        {
            ProcessingMode.ThrowIfEnumValueIsUndefined(nameof(ProcessingMode));
        }

        #endregion

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
