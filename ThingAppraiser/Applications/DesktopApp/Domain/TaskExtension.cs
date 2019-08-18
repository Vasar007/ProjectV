using System;
using System.Threading.Tasks;
using ThingAppraiser.Logging;

namespace ThingAppraiser.DesktopApp.Domain
{
    public static class TaskExtension
    {
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerWithName(nameof(TaskExtension));

        public static async void FireAndForgetSafeAsync(this Task task,
            IErrorHandler handler = null)
        {
            try
            {
                await task;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception occured during async execution.");
                handler?.HandleError(ex);
            }
        }
    }
}
