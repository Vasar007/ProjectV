using System.Net.Http;
using System.Threading.Tasks;
using Acolyte.Assertions;
using Acolyte.Common;
using ProjectV.Configuration.Options;
using ProjectV.Logging;
using ProjectV.Models.WebServices.Responses;

namespace ProjectV.Core.Net.Http
{
    public static class HttpClientExtensions
    {
        public static bool DisposeClient(this HttpClient? client,
            ProjectVServiceOptions serviceOptions)
        {
            serviceOptions.ThrowIfNull(nameof(serviceOptions));

            if (serviceOptions.DisposeHttpClient)
            {
                client?.Dispose();
                return true;
            }

            return false;
        }

        public static async Task<Result<TResponse, ErrorResponse>> SendAndReadAsync<TResponse>(
            this HttpClient client, HttpRequestMessage request, ILogger logger)
            where TResponse : class
        {
            client.ThrowIfNull(nameof(client));
            request.ThrowIfNull(nameof(request));
            logger.ThrowIfNull(nameof(logger));

            logger.Info($"Sending {request.Method} request to '{request.RequestUri}'.");

            using var response = await client.SendAsync(request);
            return await response.ReadContentAsAsync<TResponse>(logger);
        }
    }
}
