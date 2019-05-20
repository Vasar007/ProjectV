using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using ThingAppraiser.Logging;

namespace ThingAppraiser.CommunicationWebService
{
    public static class Program
    {
        /// <summary>
        /// Logger instance for current class.
        /// </summary>
        private static readonly LoggerAbstraction _logger =
            LoggerAbstraction.CreateLoggerInstanceWithName(nameof(Program));


        private static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                       .UseStartup<Startup>();
        }

        private static async Task Main(string[] args)
        {
            try
            {
                IWebHost webHost = CreateWebHostBuilder(args).Build();

                // Run the WebHost, and start accepting requests.
                // There's an async overload, so we may as well use it.
                await webHost.RunAsync();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Exception occurred in {nameof(Main)} method.");
            }
        }
    }
}
