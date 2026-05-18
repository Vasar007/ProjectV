using System;
using System.Threading;
using System.Threading.Tasks;
using Acolyte.Assertions;
using Microsoft.Extensions.DependencyInjection;
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

        public static ConfigureWebhook Create(
            IServiceProvider provider)
        {
            provider.ThrowIfNull(nameof(provider));

            var botWebhook = provider.GetRequiredService<IBotWebhook>();
            return new ConfigureWebhook(botWebhook);
        }

        #region IHostedService Implementation

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

        #endregion
    }
}
