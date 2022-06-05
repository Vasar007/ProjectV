using System.ComponentModel.DataAnnotations;
using ProjectV.Configuration;

namespace ProjectV.TelegramBotWebService.Options
{
    public sealed class BotWebhookOptions : IOptions
    {
        [Required(AllowEmptyStrings = false)]
        public string Url { get; set; } = default!;

        [Required(AllowEmptyStrings = false)]
        public string BotApiUrl { get; set; } = default!;

        public bool UseBotTokenInUrl { get; set; } = false;

        public string? CertificatePath { get; set; } = null;

        public bool? DropPendingUpdatesOnSet { get; set; } = null;

        public bool? DropPendingUpdatesOnDelete { get; set; } = null;

        public int? MaxConnections { get; set; } = null;


        public BotWebhookOptions()
        {
        }
    }
}
