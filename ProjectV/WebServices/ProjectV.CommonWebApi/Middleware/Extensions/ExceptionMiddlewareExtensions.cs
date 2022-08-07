using System;
using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using ProjectV.CommonWebApi.Models;
using ProjectV.Logging;

namespace ProjectV.CommonWebApi.Middleware.Extensions
{
    public static class ExceptionMiddlewareExtensions
    {
        /// <summary>
        /// Logger instance for current class.
        /// </summary>
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor(typeof(ExceptionMiddlewareExtensions));

        public static void ConfigureExceptionHandler(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";
                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        var ex = contextFeature.Error;
                        _logger.Error(ex, $"Something went wrong.");

                        string errorMessage = ex.GetErrorMessage();
                        string text = ErrorDetails.AsString(
                            context.Response.StatusCode, errorMessage
                        );
                        await context.Response.WriteAsync(text);
                    }
                });
            });
        }

        public static void ConfigureCustomExceptionMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
        }

        internal static string GetErrorMessage(this Exception exception)
        {
            return exception switch
            {
                AccessViolationException => RestApiStrings.AccessViolationErrorMessage,

                _ => RestApiStrings.InternalErrorMessage
            };
        }
    }
}
