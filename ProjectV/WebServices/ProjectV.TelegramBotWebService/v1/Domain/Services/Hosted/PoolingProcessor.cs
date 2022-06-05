using System;
using System.Threading;
using System.Threading.Tasks;
using Acolyte.Assertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProjectV.Logging;
using ProjectV.TelegramBotWebService.v1.Domain.Polling;

namespace ProjectV.TelegramBotWebService.v1.Domain.Services.Hosted
{
    public sealed class PoolingProcessor : BackgroundService
    {
        private static readonly ILogger _logger = LoggerFactory.CreateLoggerFor<PoolingProcessor>();

        private readonly IBotPolling _botPolling;


        public PoolingProcessor(
            IBotPolling botPolling)
        {
            _botPolling = botPolling.ThrowIfNull(nameof(botPolling));
        }

        public static PoolingProcessor Create(
            IServiceProvider provider)
        {
            provider.ThrowIfNull(nameof(provider));

            var botWebhook = provider.GetRequiredService<IBotPolling>();
            return new PoolingProcessor(botWebhook);
        }

        #region BackgroundService Overridden Methods

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.Info("Starting polling over background service.");

            await _botPolling.StartReceivingUpdatesAsync(stoppingToken);

            _logger.Info("Polling over background service has been finished.");
        }

        #endregion
    }
}
