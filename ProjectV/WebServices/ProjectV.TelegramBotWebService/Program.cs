using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using ProjectV.CommonWebApi.Service.Setup;
using ProjectV.Logging;

namespace ProjectV.TelegramBotWebService
{
    public static class Program
    {
        /// <summary>
        /// Logger instance for current class.
        /// </summary>
        private static readonly ILogger _logger = LoggerFactory.CreateLoggerFor(typeof(Program));


        private static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost
                .CreateDefaultBuilder(args)
                .UseStartup<Startup>();
        }

        private static async Task Main(string[] args)
        {
            try
            {
                _logger.PrintHeader("Telegram bot web service started.");

                var host = CreateWebHostBuilder(args).Build();
                var serviceSetup = host.Services.GetRequiredService<IServiceSetup>();

                // Execute pre-run actions.
                await using (var onRunFailAction = await serviceSetup.PreRunAsync())
                {
                    // Run the WebHost, and start accepting requests.
                    await host.RunAsync();

                    // If WebHost finished work without issues, cancel fail callback.
                    onRunFailAction.Cancel();
                }

                // Execute post-run actions.
                await serviceSetup.PostRunAsync();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Exception occurred in {nameof(Main)} method.");
            }
            finally
            {
                _logger.PrintFooter("Telegram bot web service stopped.");
            }
        }
    }
}
