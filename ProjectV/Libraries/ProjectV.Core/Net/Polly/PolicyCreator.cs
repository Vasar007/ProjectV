using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
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
            ProjectVServiceOptions serviceOptions)
        {
            return policyBuilder
                .WaitAndRetryAsync(
                    serviceOptions.HttpClientRetryCountOnFailed,
                    retryCount => serviceOptions.HttpClientRetryTimeoutOnFailed,
                    OnFailedAsync
                );
        }

        public static IAsyncPolicy<HttpResponseMessage> WaitAndRetryWithOptionsOnTimeoutExceptionAsync(
            ProjectVServiceOptions serviceOptions)
        {
            return Policy<HttpResponseMessage>
                .Handle<TimeoutException>()
                .WaitAndRetryWithOptionsAsync(serviceOptions);
        }

        private static Task OnFailedAsync(DelegateResult<HttpResponseMessage> outcome,
            TimeSpan sleepDuration, int retryCount, Context context)
        {
            _logger.LogRetryingInfo(outcome, sleepDuration, retryCount);
            return Task.CompletedTask;
        }

        public static IAsyncPolicy<HttpResponseMessage> HandleUnauthorizedAsync(
            ProjectVServiceOptions serviceOptions,
            Func<DelegateResult<HttpResponseMessage>, TimeSpan, int, Context, Task> onRetryAsync)
        {
            return Policy<HttpResponseMessage>
                .HandleResult(response => IsUnauthorized(response))
                .WaitAndRetryAsync(
                    serviceOptions.HttpClientRetryCountOnAuth,
                    retryCount => serviceOptions.HttpClientRetryTimeoutOnAuth,
                    onRetryAsync
                );
        }

        private static bool IsUnauthorized(HttpResponseMessage response)
        {
            return response.StatusCode == HttpStatusCode.Unauthorized;
        }
    }
}
