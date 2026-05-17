using System;

namespace ProjectV.DesktopApp.Domain
{
    public interface IErrorHandler
    {
        void HandleError(Exception ex);
    }
}
