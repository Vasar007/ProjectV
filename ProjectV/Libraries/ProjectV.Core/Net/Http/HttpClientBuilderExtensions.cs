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
           this IHttpClientBuilder builder, ProjectVServiceOptions serviceOptions)
        {
            builder.ThrowIfNull(nameof(builder));
            serviceOptions.ThrowIfNull(nameof(serviceOptions));

            builder
                .ConfigurePrimaryHttpMessageHandlerWithOptions(serviceOptions)
                .AddHttpErrorPoliciesWithOptions(serviceOptions)
                .AddHttpMessageHandlersWithOptions(serviceOptions); // Common handlers should be placed after Polly ones!

            return builder;
        }

        public static IHttpClientBuilder AddHttpMessageHandlersWithOptions(
           this IHttpClientBuilder builder, ProjectVServiceOptions serviceOptions)
        {
            builder.ThrowIfNull(nameof(builder));
            serviceOptions.ThrowIfNull(nameof(serviceOptions));

            builder
                .AddHttpMessageHandler(() => new HttpClientTimeoutHandler(serviceOptions));

            return builder;
        }

        public static IHttpClientBuilder ConfigurePrimaryHttpMessageHandlerWithOptions(
           this IHttpClientBuilder builder, ProjectVServiceOptions serviceOptions)
        {
            builder.ThrowIfNull(nameof(builder));
            serviceOptions.ThrowIfNull(nameof(serviceOptions));

            builder
                .ConfigurePrimaryHttpMessageHandler(() =>
                {
                    var httpClientHandler = new HttpClientHandler
                    {
                        AllowAutoRedirect = true,
                        UseCookies = true
                    };

                    if (!serviceOptions.ValidateSslCertificates)
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
            this IHttpClientBuilder builder, ProjectVServiceOptions serviceOptions)
        {
            builder.ThrowIfNull(nameof(builder));
            serviceOptions.ThrowIfNull(nameof(serviceOptions));

            builder
                .AddTransientHttpErrorPolicy(policyBuilder => policyBuilder.WaitAndRetryWithOptionsAsync(serviceOptions))
                .AddPolicyHandler(PolicyCreator.WaitAndRetryWithOptionsOnTimeoutExceptionAsync(serviceOptions));

            return builder;
        }
    }
}
