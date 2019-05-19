using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace ThingAppraiser.TelegramBotWebService
{
    public static class Program
    {
        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                       .UseStartup<Startup>();
        }

        private static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }
    }
}
