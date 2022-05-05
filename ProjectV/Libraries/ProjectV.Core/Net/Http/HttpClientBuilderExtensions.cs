using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Acolyte.Assertions;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using ProjectV.Configuration.Options;
using ProjectV.Logging;

namespace ProjectV.Core.Net.Http
{
    public static class HttpClientBuilderExtensions
    {
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor(typeof(HttpClientBuilderExtensions));

        /// <summary>
        /// Configures common project error policy for HTTP client.
        /// </summary>
        /// <param name="builder">The <see cref="IHttpClientBuilder" />.</param>
        /// <param name="serviceOptions">The service options.</param>
        /// <returns>
        /// An <see cref="IHttpClientBuilder" /> that can be used to configure the client.
        /// </returns>
        /// <remarks>
        /// Policies configured by default handle the following responses:
        /// • Network failures (as <see cref="HttpRequestException" />)<br/>
        /// • HTTP 5XX status codes (server errors)<br/>
        /// • HTTP 408 status code (request timeout)<br/>
        /// You can reconfigure this if needed.
        /// </remarks>
        public static IHttpClientBuilder AddTransientHttpErrorPolicyWithOptions(
            this IHttpClientBuilder builder, ProjectVServiceOptions serviceOptions)
        {
            builder.ThrowIfNull(nameof(builder));
            serviceOptions.ThrowIfNull(nameof(serviceOptions));

            builder
                .AddPolicyHandler(WaitAndRetryWithOptionsAsync(serviceOptions))
                .AddPolicyHandler(HandleUnauthorizedAsync(serviceOptions));

            return builder;
        }

        private static IAsyncPolicy<HttpResponseMessage> WaitAndRetryWithOptionsAsync(
            ProjectVServiceOptions serviceOptions)
        {
            return Policy<HttpResponseMessage>
                .HandleResult(response => IsFailed(response))
                .WaitAndRetryAsync(
                    serviceOptions.HttpClientRetryCountOnFailed,
                    retryCount => serviceOptions.HttpClientRetryTimeoutOnFailed,
                    OnFailedAsync
                );
        }

        private static bool IsFailed(HttpResponseMessage response)
        {
            // We handle all errors which is not unauthorized errors.
            return !IsUnauthorized(response);
        }

        private static Task OnFailedAsync(DelegateResult<HttpResponseMessage> outcome,
            TimeSpan sleepDuration, int retryCount, Context context)
        {
            LogRetryingInfo(outcome, sleepDuration, retryCount);
            return Task.CompletedTask;
        }

        private static IAsyncPolicy<HttpResponseMessage> HandleUnauthorizedAsync(
            ProjectVServiceOptions serviceOptions)
        {

            return Policy<HttpResponseMessage>
                .HandleResult(response => IsUnauthorized(response))
                .WaitAndRetryAsync(
                    serviceOptions.HttpClientRetryCountOnAuth,
                    retryCount => serviceOptions.HttpClientRetryTimeoutOnAuth,
                    RefreshAuthorizationAsync
                );
        }

        private static bool IsUnauthorized(HttpResponseMessage response)
        {
            return response.StatusCode == HttpStatusCode.Unauthorized;
        }

        private static Task RefreshAuthorizationAsync(DelegateResult<HttpResponseMessage> outcome,
            TimeSpan sleepDuration, int retryCount, Context context)
        {
            LogRetryingInfo(outcome, sleepDuration, retryCount);
            return Task.CompletedTask;
        }

        private static void LogRetryingInfo(DelegateResult<HttpResponseMessage> outcome,
           TimeSpan sleepDuration, int retryCount)
        {
            string commonPart = $"Retrying attempt {retryCount} with timeout {sleepDuration}.";

            if (outcome.Result is not null)
            {
                string statusCode = ((int)outcome.Result.StatusCode).ToString();
                string details = $"{outcome.Result.ReasonPhrase} (code: {statusCode})";
                _logger.Warn($"Request failed: {details}. {commonPart}");
            }
            else if (outcome.Exception is not null)
            {
                _logger.Warn(outcome.Exception, $"Request failed. {commonPart}");
            }
            else
            {
                _logger.Warn($"Request failed. {commonPart}");
            }
        }
    }
}
