using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using ProjectV.DataAccessLayer.EntityFramework;
using ProjectV.Logging;

namespace ProjectV.ProcessingWebService
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
            return WebHost
                .CreateDefaultBuilder(args)
                .UseStartup<Startup>();
        }

        private static async Task CreateDbIfNotExistsAsync(IWebHost webHost)
        {
            _logger.Info("Ensuring DB is existing.");

            using var scope = webHost.Services.CreateScope();
            IServiceProvider services = scope.ServiceProvider;

            try
            {
                var context = services.GetRequiredService<ProjectVDbContext>();

                bool wasCreated = await context.Database.EnsureCreatedAsync();

                string message = wasCreated
                    ? "New DB was created."
                    : "DB has already existed.";
                _logger.Info(message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occurred creating the DB.");
            }
        }

        private static async Task Main(string[] args)
        {
            try
            {
                _logger.PrintHeader("Processing web service started.");

                IWebHost webHost = CreateWebHostBuilder(args).Build();

                await CreateDbIfNotExistsAsync(webHost);

                // Run the WebHost, and start accepting requests.
                // There's an async overload, so we may as well use it.
                await webHost.RunAsync();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Exception occurred in {nameof(Main)} method.");
            }
            finally
            {
                _logger.PrintFooter("Processing web service stopped.");
            }
        }
    }
}
