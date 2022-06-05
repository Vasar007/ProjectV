using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Acolyte.Assertions;
using Acolyte.Common.Disposal;
using Microsoft.Extensions.Options;
using ProjectV.Configuration;
using ProjectV.Logging;
using ProjectV.TelegramBotWebService.Options;
using ProjectV.TelegramBotWebService.v1.Domain.Bot;
using Telegram.Bot.Types.InputFiles;

namespace ProjectV.TelegramBotWebService.v1.Domain.Webhooks
{
    public sealed class BotWebhook : IBotWebhook
    {
        private static readonly ILogger _logger = LoggerFactory.CreateLoggerFor<BotWebhook>();

        private readonly TelegramBotWebServiceOptions _options;

        private readonly IBotService _botService;

        private string FullWebhookUrl => _options.GetFullWebhookUrl();
        private string? BotCertificatePath => _options.Bot.Webhook.CertificatePath;
        private bool? DropPendingUpdatesOnSet => _options.Bot.Webhook.DropPendingUpdatesOnSet;
        private bool? DropPendingUpdatesOnDelete => _options.Bot.Webhook.DropPendingUpdatesOnDelete;
        private int? BotMaxConnections => _options.Bot.Webhook.MaxConnections;


        public BotWebhook(
            IOptions<TelegramBotWebServiceOptions> options,
            IBotService botService)
        {
            _options = options.GetCheckedValue();
            _botService = botService.ThrowIfNull(nameof(botService));
        }

        #region IBotWebhook Implementation

        public async Task<AsyncDisposableAction> SetWebhookAsync(
            CancellationToken cancellationToken)
        {
            _logger.Info($"Try to set webhook for Telegram API to {FullWebhookUrl}");

            if (!string.IsNullOrWhiteSpace(BotCertificatePath))
            {
                _logger.Info($"Trying to upload certificate additionally: [{BotCertificatePath}].");

                using var certificateFile = File.OpenRead(BotCertificatePath);
                var certificate = new InputFileStream(certificateFile);
                await SetWebhookInternalAsync(certificate, cancellationToken);
            }
            else
            {
                await SetWebhookInternalAsync(certificate: null, cancellationToken);
            }

            _logger.Info("Webhook for Telegram API was set.");

            return new AsyncDisposableAction(() => DeleteWebhookAsync(cancellationToken));
        }

        public async Task DeleteWebhookAsync(CancellationToken cancellationToken)
        {
            _logger.Info($"Try to delete webhook for Telegram API.");
            var info = await _botService.GetWebhookInfoAsync(cancellationToken);

            if (string.IsNullOrEmpty(info.Url))
            {
                _logger.Info("Webhook for Telegram API wasn't set, no need to delete.");
                return;
            }

            await _botService.DeleteWebhookAsync(
                dropPendingUpdates: DropPendingUpdatesOnDelete,
                cancellationToken: cancellationToken
            );
            _logger.Info("Webhook for Telegram API was deleted.");
        }

        #endregion

        private async Task SetWebhookInternalAsync(InputFileStream? certificate,
            CancellationToken cancellationToken)
        {
            await _botService.SetWebhookAsync(
                url: FullWebhookUrl,
                certificate: certificate,
                dropPendingUpdates: DropPendingUpdatesOnSet,
                maxConnections: BotMaxConnections,
                cancellationToken: cancellationToken
            );
        }
    }

}
