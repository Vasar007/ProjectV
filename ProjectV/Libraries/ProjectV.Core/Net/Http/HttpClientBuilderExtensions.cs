using System.Net.Http;
using Acolyte.Assertions;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using ProjectV.Configuration.Options;

namespace ProjectV.Core.Net.Http
{
    public static class HttpClientBuilderExtensions
    {
        /// <summary>
        /// Configures common project error policy for HTTP client.
        /// </summary>
        /// <param name="builder">The <see cref="IHttpClientBuilder" />.</param>
        /// <param name="serviceOptions">The service options.</param>
        /// <returns>
        /// An <see cref="IHttpClientBuilder" /> that can be used to configure the client.
        /// </returns>
        /// <remarks>
        /// Policies configured with AddTransientHttpErrorPolicy handle the following responses:
        /// <see cref="HttpRequestException" /><br/>
        /// HTTP 5xx<br/>
        /// HTTP 408<br/>
        /// </remarks>
        public static IHttpClientBuilder AddTransientHttpErrorPolicyWithOptions(
            this IHttpClientBuilder builder, ProjectVServiceOptions serviceOptions)
        {
            builder.ThrowIfNull(nameof(builder));
            serviceOptions.ThrowIfNull(nameof(serviceOptions));

            builder
                .AddTransientHttpErrorPolicy(policyBuilder => policyBuilder.WaitAndRetryAsync(serviceOptions));

            return builder;
        }

        private static IAsyncPolicy<TResult> WaitAndRetryAsync<TResult>(
            this PolicyBuilder<TResult> policyBuilder,
            ProjectVServiceOptions serviceOptions)
        {
            return policyBuilder.WaitAndRetryAsync(
                serviceOptions.HttpClientRetryCount,
                _ => serviceOptions.HttpClientRetryTimeout
            );
        }
    }
}
