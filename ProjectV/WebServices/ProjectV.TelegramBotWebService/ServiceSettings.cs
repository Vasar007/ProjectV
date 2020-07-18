namespace ProjectV.TelegramBotWebService
{
    // TODO: make this DTO immutable.
    public sealed class ServiceSettings
    {
        public string NgrokUrl { get; set; } = default!;

        public string ServiceApiUrl { get; set; } = default!;

        public string ProjectVServiceBaseAddress { get; set; } = default!;

        public string ProjectVServiceApiUrl { get; set; } = default!;


        public ServiceSettings()
        {
        }
    }
}