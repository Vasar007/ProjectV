using System.Threading.Tasks;
using Acolyte.Assertions;
using Microsoft.Extensions.Options;
using ProjectV.Logging;

namespace ProjectV.TelegramBotWebService.v1.Domain
{
    public sealed class ServiceSetup : IServiceSetup
    {
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor<UpdateService>();

        private readonly ServiceSettings _settings;

        private readonly IBotService _botService;


        public ServiceSetup(IOptions<ServiceSettings> settings, IBotService botService)
        {
            _settings = settings.Value.ThrowIfNull(nameof(settings));
            _botService = botService.ThrowIfNull(nameof(botService));
        }

        #region IServiceSetup Implementation

        public async Task<DisposableActionAsync> SetWebhookAsync()
        {
            string serviceUrl = _settings.NgrokUrl + _settings.ServiceApiUrl;

            _logger.Info($"Try to set webhook to {serviceUrl}");
            await _botService.Client.SetWebhookAsync(serviceUrl);
            _logger.Info("Webhook was set.");

            return new DisposableActionAsync(() => DeleteWebhookAsync());
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
    }
}
