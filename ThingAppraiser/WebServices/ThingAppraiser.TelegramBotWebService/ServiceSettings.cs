namespace ThingAppraiser.TelegramBotWebService
{
    // TODO: make this DTO immutable.
    public sealed class ServiceSettings
    {
        public string NgrokUrl { get; set; } = default!;

        public string ServiceApiUrl { get; set; } = default!;

        public string ThingAppraiserServiceBaseAddress { get; set; } = default!;

        public string ThingAppraiserServiceApiUrl { get; set; } = default!;


        public ServiceSettings()
        {
        }
    }
}