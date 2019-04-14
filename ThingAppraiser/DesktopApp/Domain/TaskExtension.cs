using System;
using System.Threading.Tasks;

namespace DesktopApp.Domain
{
    public static class TaskExtension
    {
        public static async void FireAndForgetSafeAsync(this Task task,
            IErrorHandler handler = null)
        {
            try
            {
                await task;
            }
            catch (Exception ex)
            {
                handler?.HandleError(ex);
            }
        }
    }
}
