using System.ComponentModel.DataAnnotations;
using Acolyte.Assertions;
using ProjectV.Configuration;

namespace ProjectV.TelegramBotWebService.Options
{
    public sealed class TelegramBotWebServiceOptions : IOptions
    {
        [Required]
        public BotOptions Bot { get; set; } = default!;

        [Required(AllowEmptyStrings = false)]
        public string ServiceApiUrl { get; set; } = default!;

        public TelegramBotWebServiceWorkingMode WorkingMode { get; set; } =
            TelegramBotWebServiceWorkingMode.Default;

        public bool IgnoreServiceSetupErrors { get; set; } = false;


        public TelegramBotWebServiceOptions()
        {
        }

        public TelegramBotWebServiceWorkingMode GetWorkingModeForDefault()
        {
            return TelegramBotWebServiceWorkingMode.WebhookViaServiceSetup;
        }

        public bool IsMode(TelegramBotWebServiceWorkingMode expectedMode)
        {
            expectedMode.ThrowIfEnumValueIsUndefined(nameof(expectedMode));

            if (WorkingMode == TelegramBotWebServiceWorkingMode.Default)
            {
                return GetWorkingModeForDefault() == expectedMode;
            }

            return WorkingMode == expectedMode;
        }

        public string GetFullWebhookUrl()
        {
            var webhookUrl = Bot.Webhook.Url;
            var serviceApiUrl = GetServiceApiUrl();

            if (!webhookUrl.EndsWith('/') && !serviceApiUrl.StartsWith('/'))
            {
                // Append exactly one slash to the start of API URL.
                serviceApiUrl = $"/{serviceApiUrl}";
            }

            return $"{webhookUrl}{serviceApiUrl}";
        }

        public string GetServiceApiUrl()
        {
            if (Bot.Webhook.UseBotTokenInUrl)
            {
                return ConstructWebhookUrlWithBotToken();
            }

            return ConstructUrlWithServiceApi();
        }

        private string ConstructWebhookUrlWithBotToken()
        {
            // Configure custom endpoint per Telegram API recommendations:
            // https://core.telegram.org/bots/api#setwebhook
            // If we'd like to make sure that the Webhook request comes from Telegram, API
            // developers recommend using a secret path in the URL: https://www.example.com/<token>.
            // Since nobody else knows our bot's token, we can be pretty sure it's Telegram API.

            var botApiUrl = Bot.Webhook.BotApiUrl;
            if (!botApiUrl.EndsWith('/'))
            {
                // Append exactly one slash to the end of API URL.
                botApiUrl = $"{botApiUrl}/";
            }

            return $"{botApiUrl}{Bot.Token}";
        }

        private string ConstructUrlWithServiceApi()
        {
            var serviceApiUrl = ServiceApiUrl;
            if (serviceApiUrl.StartsWith('/'))
            {
                // Trim exactly one slash from the start.
                serviceApiUrl = serviceApiUrl[1..];
            }

            return serviceApiUrl;
        }
    }
}
