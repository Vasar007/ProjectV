using System.ComponentModel.DataAnnotations;
using ProjectV.Configuration;

namespace ProjectV.TelegramBotWebService.Options
{
    public sealed class BotOptions : IOptions
    {
        public BotCommandsOptions Commands { get; set; } = new();

        public string Token { get; } =
            EnvironmentVariablesParser.GetValueOrDefault("BotToken", string.Empty);

        [Required(AllowEmptyStrings = false)]
        public string WebhookUrl { get; set; } = default!;

        [Required(AllowEmptyStrings = false)]
        public string BotWebhookApiUrl { get; set; } = default!;

        public bool UseBotTokenInWebhookUrl { get; set; } = false;

        public string? CertificatePath { get; set; } = null;

        public bool? DropPendingUpdatesOnSet { get; set; } = null;

        public bool? DropPendingUpdatesOnDelete { get; set; } = null;

        public int? MaxConnections { get; set; } = null;

        [Required(AllowEmptyStrings = false)]
        public string NewLineSeparator { get; set; } = default!;


        public BotOptions()
        {
        }
    }
}
