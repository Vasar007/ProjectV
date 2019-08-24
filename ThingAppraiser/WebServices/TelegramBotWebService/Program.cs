using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using ThingAppraiser.Logging;
using ThingAppraiser.TelegramBotWebService.v1.Domain;

namespace ThingAppraiser.TelegramBotWebService
{
    public static class Program
    {
        /// <summary>
        /// Logger instance for current class.
        /// </summary>
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor(typeof(Program));


        private static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                       .UseStartup<Startup>();
        }

        private static async Task Main(string[] args)
        {
            try
            {
                _logger.PrintHeader("Telegram bot web service started.");

                IWebHost webHost = CreateWebHostBuilder(args).Build();

                // Set web hook to get messages from Telegram Bot.
                var serviceSetup = webHost.Services.GetRequiredService<IServiceSetupAsync>();
                await serviceSetup.SetWebhookAsync();

                // Run the WebHost, and start accepting requests.
                // There's an async overload, so we may as well use it.
                await webHost.RunAsync();

                await serviceSetup.DeleteWebhookAsync();
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
