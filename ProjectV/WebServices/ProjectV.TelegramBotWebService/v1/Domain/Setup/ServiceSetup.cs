using System.IO;
using System.Threading.Tasks;
using Acolyte.Assertions;
using Acolyte.Common.Disposal;
using Microsoft.Extensions.Options;
using ProjectV.Configuration;
using ProjectV.Logging;
using ProjectV.TelegramBotWebService.Options;
using ProjectV.TelegramBotWebService.v1.Domain.Bot;
using Telegram.Bot.Types.InputFiles;

namespace ProjectV.TelegramBotWebService.v1.Domain.Setup
{
    public sealed class ServiceSetup : IServiceSetup
    {
        private static readonly ILogger _logger = LoggerFactory.CreateLoggerFor<ServiceSetup>();

        private readonly TelegramBotWebServiceOptions _options;

        private readonly IBotService _botService;

        private string FullWebhookServiceUrl => $"{_options.Bot.WebhookUrl}{_options.ServiceApiUrl}";
        private string? BotCertificatePath => _options.Bot.CertificatePath;
        private bool? BotDropPendingUpdates => _options.Bot.DropPendingUpdates;
        private int? BotMaxConnections => _options.Bot.MaxConnections;


        public ServiceSetup(
            IOptions<TelegramBotWebServiceOptions> options,
            IBotService botService)
        {
            _options = options.GetCheckedValue();
            _botService = botService.ThrowIfNull(nameof(botService));
        }

        #region IServiceSetup Implementation

        public async Task<AsyncDisposableAction> SetWebhookAsync()
        {
            _logger.Info($"Try to set webhook to {FullWebhookServiceUrl}");

            if (!string.IsNullOrWhiteSpace(BotCertificatePath))
            {
                _logger.Info($"Trying to upload certificate additionally {BotCertificatePath}");

                using var certificateFile = File.OpenRead(BotCertificatePath);
                var certificate = new InputFileStream(certificateFile);
                await SetWebhookInternalAsync(certificate);
            }
            else
            {
                await SetWebhookInternalAsync(certificate: null);
            }

            _logger.Info("Webhook was set.");

            return new AsyncDisposableAction(() => DeleteWebhookAsync());
        }

        public async Task DeleteWebhookAsync()
        {
            _logger.Info($"Try to delete webhook.");
            var info = await _botService.GetWebhookInfoAsync();

            if (string.IsNullOrEmpty(info.Url))
            {
                _logger.Info("Webhook wasn't set, no need to delete.");
                return;
            }

            await _botService.DeleteWebhookAsync();
            _logger.Info("Webhook was deleted.");
        }

        #endregion

        private async Task SetWebhookInternalAsync(InputFileStream? certificate)
        {
            await _botService.SetWebhookAsync(
                url: FullWebhookServiceUrl,
                certificate: certificate,
                dropPendingUpdates: BotDropPendingUpdates,
                maxConnections: BotMaxConnections
            );
        }
    }
}
