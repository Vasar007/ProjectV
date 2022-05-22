using System.IO;
using System.Threading.Tasks;
using Acolyte.Assertions;
using Acolyte.Common.Disposal;
using Microsoft.Extensions.Options;
using ProjectV.Logging;
using ProjectV.TelegramBotWebService.Config;
using ProjectV.TelegramBotWebService.v1.Domain.Bot;
using Telegram.Bot;
using Telegram.Bot.Types.InputFiles;

namespace ProjectV.TelegramBotWebService.v1.Domain.Setup
{
    public sealed class ServiceSetup : IServiceSetup
    {
        private static readonly ILogger _logger = LoggerFactory.CreateLoggerFor<ServiceSetup>();

        private readonly TelegramBotWebServiceSettings _settings;

        private readonly IBotService _botService;


        public ServiceSetup(
            IOptions<TelegramBotWebServiceSettings> settings,
            IBotService botService)
        {
            _settings = settings.Value.ThrowIfNull(nameof(settings));
            _botService = botService.ThrowIfNull(nameof(botService));
        }

        #region IServiceSetup Implementation

        public async Task<AsyncDisposableAction> SetWebhookAsync()
        {
            string serviceUrl = $"{_settings.WebhookUrl}{_settings.ServiceApiUrl}";

            _logger.Info($"Try to set webhook to {serviceUrl}");

            var certificatePath = _settings.SslCertificatePath;
            if (!string.IsNullOrWhiteSpace(certificatePath))
            {
                _logger.Info($"Trying to upload certificate additionally {certificatePath}");

                using var certificateFile = File.OpenRead(certificatePath);
                var certificate = new InputFileStream(certificateFile);
                await SetWebhookInternalAsync(serviceUrl, certificate);
            }
            else
            {
                await SetWebhookInternalAsync(serviceUrl, certificate: null);
            }

            _logger.Info("Webhook was set.");

            return new AsyncDisposableAction(() => DeleteWebhookAsync());
        }

        public async Task DeleteWebhookAsync()
        {
            _logger.Info($"Try to delete webhook.");
            var info = await _botService.Client.GetWebhookInfoAsync();

            if (!string.IsNullOrEmpty(info.Url))
            {
                await _botService.Client.DeleteWebhookAsync();
                _logger.Info("Webhook was deleted.");
            }

            _logger.Info("Webhook wasn't set, no need to delete.");
        }

        #endregion

        private async Task SetWebhookInternalAsync(string serviceUrl, InputFileStream? certificate)
        {
            await _botService.Client.SetWebhookAsync(
                url: serviceUrl,
                certificate: certificate,
                dropPendingUpdates: _settings.DropPendingUpdates,
                maxConnections: _settings.MaxConnections
            );
        }
    }
}
