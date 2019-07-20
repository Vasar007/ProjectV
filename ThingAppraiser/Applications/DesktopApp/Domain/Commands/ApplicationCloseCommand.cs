using System.Windows;

namespace ThingAppraiser.DesktopApp.Domain.Commands
{
    internal static class ApplicationCloseCommand
    {
#pragma warning disable IDE0060 // Remove unused parameter
        public static bool CanExecute(object parameter)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            return !(Application.Current is null) && !(Application.Current.MainWindow is null);
        }

#pragma warning disable IDE0060 // Remove unused parameter
        public static void Execute(object parameter)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            Application.Current.MainWindow.Close();

            // If no need to handle closing event, use this:
            // Application.Current.Shutdown();
        }
    }
}
