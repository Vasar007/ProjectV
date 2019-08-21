namespace ThingAppraiser.TelegramBotWebService
{
    public sealed class ServiceSettings
    {
        public string NgrokUrl { get; set; }

        public string ServiceApiUrl { get; set; }

        public string ThingAppraiserServiceBaseAddress { get; set; }

        public string ThingAppraiserServiceApiUrl { get; set; }


        public ServiceSettings()
        {
        }
    }
}