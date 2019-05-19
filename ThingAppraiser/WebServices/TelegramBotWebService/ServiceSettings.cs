namespace ThingAppraiser.TelegramBotWebService
{
    public class ServiceSettings
    {
        public string TelegramApiUrl { get; set; }

        public string SetWebhookRequestUrlTemplate { get; set; }

        public string NgrokUrl { get; set; }

        public string ServiceApiUrl { get; set; }


        public ServiceSettings()
        {
        }
    }
}