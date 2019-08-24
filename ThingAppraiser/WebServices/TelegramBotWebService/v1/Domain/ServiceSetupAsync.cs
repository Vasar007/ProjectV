using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using ThingAppraiser.Logging;

namespace ThingAppraiser.TelegramBotWebService.v1.Domain
{
    public sealed class ServiceSetupAsync : IServiceSetupAsync
    {
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor<UpdateServiceAsync>();

        private readonly ServiceSettings _settings;

        private readonly IBotService _botService;


        public ServiceSetupAsync(IOptions<ServiceSettings> settings, IBotService botService)
        {
            _settings = settings.Value.ThrowIfNull(nameof(settings));
            _botService = botService.ThrowIfNull(nameof(botService));
        }

        #region IServiceSetupAsync Implementation

        public async Task SetWebhookAsync()
        {
            string serviceUrl = _settings.NgrokUrl + _settings.ServiceApiUrl;

            _logger.Info($"Try to set webhook to {serviceUrl}");
            await _botService.Client.SetWebhookAsync(serviceUrl);
            _logger.Info("Webhook was set.");
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
