using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Acolyte.Assertions;
using Polly;
using ProjectV.Configuration.Options;
using ProjectV.Core.Logging;
using ProjectV.Logging;

namespace ProjectV.Core.Net.Polly
{
    public static class PolicyCreator
    {
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor(typeof(PolicyCreator));

        public static IAsyncPolicy<HttpResponseMessage> WaitAndRetryWithOptionsAsync(
            this PolicyBuilder<HttpResponseMessage> policyBuilder,
            HttpClientOptions options)
        {
            policyBuilder.ThrowIfNull(nameof(policyBuilder));
            options.ThrowIfNull(nameof(options));

            return policyBuilder
                .WaitAndRetryAsync(
                    options.RetryCountOnFailed,
                    retryCount => options.RetryTimeoutOnFailed,
                    OnFailedAsync
                );
        }

        public static IAsyncPolicy<HttpResponseMessage> WaitAndRetryWithOptionsOnTimeoutExceptionAsync(
            HttpClientOptions options)
        {
            options.ThrowIfNull(nameof(options));

            return Policy<HttpResponseMessage>
                .Handle<TimeoutException>()
                .WaitAndRetryWithOptionsAsync(options);
        }

        private static Task OnFailedAsync(DelegateResult<HttpResponseMessage> outcome,
            TimeSpan sleepDuration, int retryCount, Context context)
        {
            _logger.LogRetryingInfo(outcome, sleepDuration, retryCount);
            return Task.CompletedTask;
        }

        public static IAsyncPolicy<HttpResponseMessage> HandleUnauthorizedAsync(
            HttpClientOptions options,
            Func<DelegateResult<HttpResponseMessage>, TimeSpan, int, Context, Task> onRetryAsync)
        {
            options.ThrowIfNull(nameof(options));
            onRetryAsync.ThrowIfNull(nameof(onRetryAsync));

            return Policy<HttpResponseMessage>
                .HandleResult(response => IsUnauthorized(response))
                .WaitAndRetryAsync(
                    options.RetryCountOnAuth,
                    retryCount => options.RetryTimeoutOnAuth,
                    onRetryAsync
                );
        }

        private static bool IsUnauthorized(HttpResponseMessage response)
        {
            return response.StatusCode == HttpStatusCode.Unauthorized;
        }
    }
}
