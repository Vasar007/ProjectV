namespace ProjectV.TelegramBotWebService.Options
{
    public enum BotPollingProcessingMode
    {
        Default = 0,

        LoopReceiving = 1,
        AsyncQueuedReceiving = 2,
        AsyncBlockingReceiving = 3
    }
}
