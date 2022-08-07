using System.ComponentModel.DataAnnotations;
using Acolyte.Assertions;
using ProjectV.Configuration;

namespace ProjectV.TelegramBotWebService.Options
{
    public sealed class BotOptions : IOptions
    {
        public BotCommandsOptions Commands { get; init; } = new();

        public BotWebhookOptions Webhook { get; init; } = new();

        public BotPollingOptions Polling { get; init; } = new();

        public string Token { get; init; } =
            EnvironmentVariablesParser.GetValueOrDefault("BotToken", string.Empty);

        [Required(AllowEmptyStrings = false)]
        public string NewLineSeparator { get; init; } = default!;


        public BotOptions()
        {
        }

        #region IOptions Implementation

        public void Validate()
        {
            Commands.ThrowIfNull(nameof(Commands));
            Webhook.ThrowIfNull(nameof(Webhook));
            Polling.ThrowIfNull(nameof(Polling));

            Token.ThrowIfNullOrWhiteSpace(nameof(Token));
            NewLineSeparator.ThrowIfNullOrWhiteSpace(nameof(NewLineSeparator));
        }

        #endregion
    }
}
