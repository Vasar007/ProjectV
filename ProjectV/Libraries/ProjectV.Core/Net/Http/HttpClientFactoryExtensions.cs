using System;
using System.Net.Http;
using Acolyte.Assertions;
using ProjectV.Configuration.Options;
using ProjectV.Logging;

namespace ProjectV.Core.Net.Http
{
    public static class HttpClientFactoryExtensions
    {
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor(typeof(HttpClientExtensions));

        public static HttpClient CreateClientWithOptions(this IHttpClientFactory httpClientFactory,
            string baseAddress, ProjectVServiceOptions serviceOptions)
        {
            httpClientFactory.ThrowIfNull(nameof(httpClientFactory));
            baseAddress.ThrowIfNull(nameof(baseAddress));
            serviceOptions.ThrowIfNull(nameof(serviceOptions));

            string defaultClientName = serviceOptions.HttpClientDefaultName;
            _logger.Info($"Using client '{defaultClientName}' and service URL: {baseAddress}");

            HttpClient client = httpClientFactory.CreateClient(defaultClientName);
            try
            {
                client.ConfigureWithJsonMedia(baseAddress, serviceOptions);

                return client;
            }
            catch (Exception)
            {
                client.DisposeClient(serviceOptions);
                throw;
            }
        }
    }
}
