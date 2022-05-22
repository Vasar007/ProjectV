using System;
using System.Net.Http;
using Acolyte.Assertions;
using Acolyte.Common;
using Acolyte.Common.Monads;
using ProjectV.Configuration.Options;
using ProjectV.Logging;
using ProjectV.Options;

namespace ProjectV.Core.Net.Http
{
    public static class HttpClientFactoryExtensions
    {
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor(typeof(HttpClientFactoryExtensions));


        public static HttpClient CreateClientWithOptions(this IHttpClientFactory httpClientFactory,
            HttpClientOptions options)
        {
            httpClientFactory.ThrowIfNull(nameof(httpClientFactory));
            options.ThrowIfNull(nameof(options));

            return httpClientFactory.CreateClientWithOptions(baseAddress: null, options);
        }

        public static HttpClient CreateClientWithOptions(this IHttpClientFactory httpClientFactory,
            string? baseAddress, HttpClientOptions options)
        {
            httpClientFactory.ThrowIfNull(nameof(httpClientFactory));
            options.ThrowIfNull(nameof(options));

            string defaultClientName = options.HttpClientDefaultName;
            string serviceUrl = baseAddress.ToStringNullSafe(CommonConstants.NotAvailable);
            _logger.Info($"Using client '{defaultClientName}' and service URL: {serviceUrl}");

            HttpClient client = httpClientFactory.CreateClient(defaultClientName);
            try
            {
                return client
                    .ApplyIf(!string.IsNullOrWhiteSpace(baseAddress), c => c.ConfigureBaseAddress(baseAddress!))
                    .ConfigureWithOptions(options)
                    .ConfigureWithJsonMedia();
            }
            catch (Exception)
            {
                client.DisposeClient(options);
                throw;
            }
        }
    }
}
