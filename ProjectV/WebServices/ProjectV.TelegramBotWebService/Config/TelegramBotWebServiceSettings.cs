namespace ProjectV.TelegramBotWebService.Config
{
    // TODO: make this DTO immutable.
    public sealed class TelegramBotWebServiceSettings
    {
        public string WebhookUrl { get; set; } = default!;

        public string ServiceApiUrl { get; set; } = default!;

        public string? SslCertificatePath { get; set; } = null;

        public bool? DropPendingUpdates { get; set; } = null;

        public int? MaxConnections { get; set; } = null;

        public string EndlineSeparator { get; set; } = default!;


        public TelegramBotWebServiceSettings()
        {
        }
    }
}
