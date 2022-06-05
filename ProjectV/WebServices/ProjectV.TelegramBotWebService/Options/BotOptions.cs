using System.ComponentModel.DataAnnotations;
using ProjectV.Configuration;

namespace ProjectV.TelegramBotWebService.Options
{
    public sealed class BotOptions : IOptions
    {
        public BotCommandsOptions Commands { get; set; } = new();

        public BotWebhookOptions Webhook { get; set; } = new();

        public BotPollingOptions Polling { get; set; } = new();

        public string Token { get; } =
            EnvironmentVariablesParser.GetValueOrDefault("BotToken", string.Empty);

        [Required(AllowEmptyStrings = false)]
        public string NewLineSeparator { get; set; } = default!;


        public BotOptions()
        {
        }
    }
}
