using System.Net.Http;
using Acolyte.Assertions;
using Microsoft.Extensions.DependencyInjection;
using ProjectV.Configuration.Options;
using ProjectV.Core.Net.Polly;
using ProjectV.Logging;

namespace ProjectV.Core.Net.Http
{
    public static class HttpClientBuilderExtensions
    {
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor(typeof(HttpClientBuilderExtensions));

        public static IHttpClientBuilder AddHttpOptions(
           this IHttpClientBuilder builder, HttpClientOptions options)
        {
            builder.ThrowIfNull(nameof(builder));
            options.ThrowIfNull(nameof(options));

            builder
                .ConfigurePrimaryHttpMessageHandlerWithOptions(options)
                .AddHttpErrorPoliciesWithOptions(options)
                .AddHttpMessageHandlersWithOptions(options); // Common handlers should be placed after Polly ones!

            return builder;
        }

        public static IHttpClientBuilder AddHttpMessageHandlersWithOptions(
           this IHttpClientBuilder builder, HttpClientOptions options)
        {
            builder.ThrowIfNull(nameof(builder));
            options.ThrowIfNull(nameof(options));

            builder
                .AddHttpMessageHandler(() => new HttpClientTimeoutHandler(options));

            return builder;
        }

        public static IHttpClientBuilder ConfigurePrimaryHttpMessageHandlerWithOptions(
           this IHttpClientBuilder builder, HttpClientOptions options)
        {
            builder.ThrowIfNull(nameof(builder));
            options.ThrowIfNull(nameof(options));

            builder
                .ConfigurePrimaryHttpMessageHandler(() =>
                {
                    var httpClientHandler = new HttpClientHandler
                    {
                        AllowAutoRedirect = true,
                        UseCookies = true
                    };

                    if (!options.ValidateSslCertificates)
                    {
                        _logger.Warn("ATTENTION! SSL certificates validation is disabled.");
                        httpClientHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
                    }

                    return httpClientHandler;
                });

            return builder;
        }

        /// <summary>
        /// Configures common project error policy for HTTP client.
        /// </summary>
        /// <param name="builder">The <see cref="IHttpClientBuilder" />.</param>
        /// <param name="serviceOptions">The service options.</param>
        /// <returns>
        /// An <see cref="IHttpClientBuilder" /> that can be used to configure the client.
        /// </returns>
        /// <remarks>
        /// Policies configured by AddTransientHttpErrorPolicy handle the following responses:
        /// • Network failures (as <see cref="HttpRequestException" />)<br/>
        /// • HTTP 5XX status codes (server errors)<br/>
        /// • HTTP 408 status code (request timeout)<br/>
        /// Or you can create custom policy and add it by AddPolicyHandler.
        /// </remarks>
        public static IHttpClientBuilder AddHttpErrorPoliciesWithOptions(
            this IHttpClientBuilder builder, HttpClientOptions options)
        {
            builder.ThrowIfNull(nameof(builder));
            options.ThrowIfNull(nameof(options));

            builder
                .AddTransientHttpErrorPolicy(policyBuilder => policyBuilder.WaitAndRetryWithOptionsAsync(options))
                .AddPolicyHandler(PolicyCreator.WaitAndRetryWithOptionsOnTimeoutExceptionAsync(options));

            return builder;
        }
    }
}
