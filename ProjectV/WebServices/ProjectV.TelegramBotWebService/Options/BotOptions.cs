using System.ComponentModel.DataAnnotations;
using ProjectV.Configuration;

namespace ProjectV.TelegramBotWebService.Options
{
    public sealed class BotOptions : IOptions
    {
        public string Token { get; } =
            EnvironmentVariablesParser.GetValueOrDefault("BotToken", "BOT_TOKEN");

        [Required(AllowEmptyStrings = false)]
        public string WebhookUrl { get; set; } = default!;

        public string? SslCertificatePath { get; set; } = null;

        public bool? DropPendingUpdates { get; set; } = null;

        public int? MaxConnections { get; set; } = null;

        [Required(AllowEmptyStrings = false)]
        public string NewLineSeparator { get; set; } = default!;


        public BotOptions()
        {
        }
    }
}
