namespace ProjectV.TelegramBotWebService.Options
{
    public enum TelegramBotWebServiceWorkingMode
    {
        Default = 0,

        WebhookViaServiceSetup = 1,
        WebhookViaHostedService = 2,

        PollingViaHostedService = 3
    }
}
