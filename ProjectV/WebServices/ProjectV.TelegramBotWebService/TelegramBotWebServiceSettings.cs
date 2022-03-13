namespace ProjectV.TelegramBotWebService
{
    // TODO: make this DTO immutable.
    public sealed class TelegramBotWebServiceSettings
    {
        public string NgrokUrl { get; set; } = default!;

        public string ServiceApiUrl { get; set; } = default!;

        public string ProjectVServiceBaseAddress { get; set; } = default!;

        public string ProjectVServiceApiUrl { get; set; } = default!;

        public string EndlineSeparator { get; set; } = default!;


        public TelegramBotWebServiceSettings()
        {
        }
    }
}
