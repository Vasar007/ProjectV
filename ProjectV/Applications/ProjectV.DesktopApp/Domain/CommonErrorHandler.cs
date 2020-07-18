using System;
using ProjectV.Logging;

namespace ProjectV.DesktopApp.Domain
{
    public sealed class CommonErrorHandler : IErrorHandler
    {
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor<CommonErrorHandler>();


        public CommonErrorHandler()
        {
        }

        #region IErrorHandler Implementation

        public void HandleError(Exception ex)
        {
            _logger.Error(ex, "Exception occured during task execution.");
        }

        #endregion
    }
}
