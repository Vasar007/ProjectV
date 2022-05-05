using System;
using System.Net.Http;
using System.Net.Http.Headers;
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
            ProjectVServiceOptions serviceOptions)
        {
            httpClientFactory.ThrowIfNull(nameof(httpClientFactory));
            serviceOptions.ThrowIfNull(nameof(serviceOptions));

            string baseAddress = serviceOptions.CommunicationServiceBaseAddress;
            string defaultClientName = serviceOptions.HttpClientDefaultName;
            _logger.Info($"Using client '{defaultClientName}' and service URL: {baseAddress}");

            HttpClient client = httpClientFactory.CreateClient(defaultClientName);
            try
            {
                client.BaseAddress = new Uri(baseAddress);
                client.Timeout = serviceOptions.HttpClientRetryTimeoutOnRequest;

                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // TODO: add option to login for user and use user's access token.
                if (!string.IsNullOrWhiteSpace(serviceOptions.AccessToken))
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {serviceOptions.AccessToken}");
                }

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
