using System;
using System.Net.Http;
using Acolyte.Assertions;
using Polly;
using ProjectV.Logging;

namespace ProjectV.Core.Logging
{
    public static class LoggerExtensions
    {
        public static void LogRetryingInfo(this ILogger logger,
            DelegateResult<HttpResponseMessage> outcome, TimeSpan sleepDuration, int retryCount)
        {
            logger.ThrowIfNull(nameof(logger));
            outcome.ThrowIfNull(nameof(outcome));

            string commonPart = $"Retrying attempt {retryCount} with timeout {sleepDuration}.";

            if (outcome.Result is not null)
            {
                string statusCode = ((int) outcome.Result.StatusCode).ToString();
                string details = $"{outcome.Result.ReasonPhrase} (code: {statusCode})";
                logger.Warn($"Request failed: {details}. {commonPart}");
            }
            else if (outcome.Exception is not null)
            {
                logger.Warn(outcome.Exception, $"Request failed. {commonPart}");
            }
            else
            {
                logger.Warn($"Request failed. {commonPart}");
            }
        }
    }
}
