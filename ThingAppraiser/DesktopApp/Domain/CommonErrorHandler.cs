using System;
using ThingAppraiser.Logging;

namespace DesktopApp.Domain
{
    public class CommonErrorHandler : IErrorHandler
    {
        private static readonly CLoggerAbstraction s_logger =
            CLoggerAbstraction.CreateLoggerInstanceFor<CommonErrorHandler>();


        public CommonErrorHandler()
        {
        }

        #region IErrorHandler Implementation

        public void HandleError(Exception ex)
        {
            s_logger.Error(ex, "Exception occured during task execution.");
        }

        #endregion
    }
}
