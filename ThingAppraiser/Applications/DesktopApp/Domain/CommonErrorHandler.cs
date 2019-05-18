using System;
using ThingAppraiser.Logging;

namespace ThingAppraiser.DesktopApp.Domain
{
    public class CommonErrorHandler : IErrorHandler
    {
        private static readonly LoggerAbstraction _logger =
            LoggerAbstraction.CreateLoggerInstanceFor<CommonErrorHandler>();


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
