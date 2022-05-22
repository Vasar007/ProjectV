using ProjectV.Configuration;

namespace ProjectV.TelegramBotWebService.Options
{
    // TODO: make this DTO immutable.
    public sealed class TelegramBotWebServiceOptions : IOptions
    {
        public string WebhookUrl { get; set; } = default!;

        public string ServiceApiUrl { get; set; } = default!;

        public string? SslCertificatePath { get; set; } = null;

        public bool? DropPendingUpdates { get; set; } = null;

        public int? MaxConnections { get; set; } = null;

        public string NewLineSeparator { get; set; } = default!;


        public TelegramBotWebServiceOptions()
        {
        }
    }
}
