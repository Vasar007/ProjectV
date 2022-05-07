using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
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

            return client.DisposeClient(serviceOptions.ShouldDisposeHttpClient);
        }

        public static bool DisposeClient(this HttpClient? client,
            bool shouldDisposeHttpClient)
        {
            if (shouldDisposeHttpClient)
            {
                client?.Dispose();
                return true;
            }

            return false;
        }

        public static HttpClient ConfigureWithJsonMedia(this HttpClient client, string baseAddress,
            ProjectVServiceOptions serviceOptions)
        {
            client.ThrowIfNull(nameof(client));
            baseAddress.ThrowIfNull(nameof(baseAddress));
            serviceOptions.ThrowIfNull(nameof(serviceOptions));

            client.BaseAddress = new Uri(baseAddress);
            client.Timeout = serviceOptions.HttpClientTimeoutOnRequest;

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }

        public static Task<Result<TResponse, ErrorResponse>> SendAndReadAsync<TResponse>(
            this HttpClient client, HttpRequestMessage request, ILogger logger)
            where TResponse : class
        {
            return client.SendAndReadAsync<TResponse>(
                request,
                logger,
                continueOnCapturedContext: false,
                cancellationToken: CancellationToken.None
            );
        }

        public static async Task<Result<TResponse, ErrorResponse>> SendAndReadAsync<TResponse>(
            this HttpClient client, HttpRequestMessage request, ILogger logger,
            bool continueOnCapturedContext, CancellationToken cancellationToken)
            where TResponse : class
        {
            client.ThrowIfNull(nameof(client));
            request.ThrowIfNull(nameof(request));
            logger.ThrowIfNull(nameof(logger));

            logger.Info($"Sending {request.Method} request to '{request.RequestUri}'.");

            using var response = await client.SendAsync(request, cancellationToken)
                .ConfigureAwait(continueOnCapturedContext);

            return await response.ReadContentAsAsync<TResponse>(
                    logger, continueOnCapturedContext, cancellationToken
                )
                .ConfigureAwait(continueOnCapturedContext);
        }
    }
}
