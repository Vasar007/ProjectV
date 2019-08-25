using System;

namespace ThingAppraiser.DesktopApp.Domain
{
    public interface IErrorHandler
    {
        void HandleError(Exception ex);
    }
}
