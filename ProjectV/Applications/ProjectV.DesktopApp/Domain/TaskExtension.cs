using System;
using System.Threading.Tasks;
using ProjectV.Logging;

namespace ProjectV.DesktopApp.Domain
{
    public static class TaskExtension
    {
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor(typeof(TaskExtension));

        public static async void FireAndForgetSafeAsync(this Task task,
            IErrorHandler? handler = null)
        {
            handler ??= new CommonErrorHandler();

            try
            {
                await task;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception occured during async execution.");
                handler.HandleError(ex);
            }
        }
    }
}
