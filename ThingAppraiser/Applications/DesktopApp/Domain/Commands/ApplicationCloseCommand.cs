using System.Windows;

namespace ThingAppraiser.DesktopApp.Domain.Commands
{
    internal static class ApplicationCloseCommand
    {
        public static bool CanExecute(object parameter)
        {
            return !(Application.Current is null) && !(Application.Current.MainWindow is null);
        }

        public static void Execute(object parameter)
        {
            Application.Current.MainWindow.Close();

            // If no need to handle closing event, use this:
            // Application.Current.Shutdown();
        }
    }
}
