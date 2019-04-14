using System;

namespace DesktopApp.Domain
{
    public interface IErrorHandler
    {
        void HandleError(Exception ex);
    }
}
