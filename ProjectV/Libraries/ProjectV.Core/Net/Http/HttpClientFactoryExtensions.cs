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
            LoggerFactory.CreateLoggerFor(typeof(HttpClientFactoryExtensions));

        public static HttpClient CreateClientWithOptions(this IHttpClientFactory httpClientFactory,
            string baseAddress, HttpClientOptions options)
        {
            httpClientFactory.ThrowIfNull(nameof(httpClientFactory));
            baseAddress.ThrowIfNull(nameof(baseAddress));
            options.ThrowIfNull(nameof(options));

            string defaultClientName = options.HttpClientDefaultName;
            _logger.Info($"Using client '{defaultClientName}' and service URL: {baseAddress}");

            HttpClient client = httpClientFactory.CreateClient(defaultClientName);
            try
            {
                return client
                    .ConfigureWithJsonMedia(baseAddress)
                    .ConfigureWithOptions(options);
            }
            catch (Exception)
            {
                client.DisposeClient(options);
                throw;
            }
        }
    }
}
