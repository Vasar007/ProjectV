using System.ComponentModel.DataAnnotations;
using Acolyte.Assertions;
using ProjectV.Configuration;

namespace ProjectV.TelegramBotWebService.Options
{
    public sealed class BotWebhookOptions : IOptions
    {
        [Required(AllowEmptyStrings = false)]
        public string Url { get; init; } = default!;

        [Required(AllowEmptyStrings = false)]
        public string BotApiUrl { get; init; } = default!;

        public bool UseBotTokenInUrl { get; init; } = false;

        public string? CertificatePath { get; init; } = null;

        public bool DropPendingUpdatesOnSet { get; init; } = false;

        public bool DropPendingUpdatesOnDelete { get; init; } = false;

        public int? MaxConnections { get; init; } = null;


        public BotWebhookOptions()
        {
        }

        #region IOptions Implementation

        public void Validate()
        {
            Url.ThrowIfNullOrWhiteSpace(nameof(Url));

            if (UseBotTokenInUrl)
            {
                BotApiUrl.ThrowIfNullOrWhiteSpace(nameof(BotApiUrl));
            }
        }

        #endregion
    }
}
