using System;
using System.Threading;
using System.Threading.Tasks;
using Acolyte.Assertions;
using Microsoft.Extensions.Hosting;
using ProjectV.Logging;

namespace ProjectV.CommonWebApi.Service.Hosted
{
    public sealed class HostedServiceSafeWrapper : IHostedService
    {
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor<HostedServiceSafeWrapper>();

        private readonly IHostedService _realHostedService;


        public HostedServiceSafeWrapper(
            IHostedService realHostedService)
        {
            _realHostedService = realHostedService.ThrowIfNull(nameof(realHostedService));
        }

        #region IHostedService Implementation

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.Info("Starting hosted service with safe wrapper.");

            try
            {
                await _realHostedService.StartAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to start hosted service.");
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.Info("Stopping hosted service with safe wrapper.");

            try
            {
                await _realHostedService.StopAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to stop hosted service.");
            }
        }

        #endregion
    }
}
