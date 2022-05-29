using System;
using System.Net;
using System.Threading.Tasks;
using Acolyte.Assertions;
using Microsoft.AspNetCore.Http;
using ProjectV.CommonWebApi.Middleware.Extensions;
using ProjectV.CommonWebApi.Models;
using ProjectV.Logging;

namespace ProjectV.CommonWebApi.Middleware
{
    public sealed class ExceptionMiddleware
    {
        /// <summary>
        /// Logger instance for current class.
        /// </summary>
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor<ExceptionMiddleware>();

        private readonly RequestDelegate _next;


        public ExceptionMiddleware(
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
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            string message = exception.GetErrorMessage();

            string text = ErrorDetails.AsString(context.Response.StatusCode, message);
            await context.Response.WriteAsync(text);
        }
    }
}
