using System.Threading;
using System.Threading.Tasks;
using Acolyte.Assertions;
using Microsoft.Extensions.Hosting;
using ProjectV.Logging;
using ProjectV.TelegramBotWebService.v1.Domain.Webhooks;

namespace ProjectV.TelegramBotWebService.v1.Domain.Services.Hosted
{
    public sealed class ConfigureWebhook : IHostedService
    {
        private static readonly ILogger _logger = LoggerFactory.CreateLoggerFor<ConfigureWebhook>();

        private readonly IBotWebhook _botWebhook;


        public ConfigureWebhook(
            IBotWebhook botWebhook)
        {
            _botWebhook = botWebhook.ThrowIfNull(nameof(botWebhook));
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.Info("Setting webhook over hosted service.");

            await _botWebhook.SetWebhookAsync(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.Info("Deleting webhook over hosted service.");

            await _botWebhook.DeleteWebhookAsync(cancellationToken);
        }
    }
}
