using System;
using System.Threading.Tasks;
using Acolyte.Assertions;
using Microsoft.AspNetCore.Http;
using ProjectV.Logging;

namespace ProjectV.CommonWebApi
{
    public sealed class ErrorLoggingMiddleware
    {
        /// <summary>
        /// Logger instance for current class.
        /// </summary>
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor<ErrorLoggingMiddleware>();

        private readonly RequestDelegate _next;

        public ErrorLoggingMiddleware(
            RequestDelegate next)
        {
            _next = next.ThrowIfNull(nameof(next));
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Request failed with an error.");
                throw;
            }
        }
    }
}
