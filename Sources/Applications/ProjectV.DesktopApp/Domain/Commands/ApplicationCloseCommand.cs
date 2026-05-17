using System.Windows;

namespace ProjectV.DesktopApp.Domain.Commands
{
    internal static class ApplicationCloseCommand
    {
        public static bool CanExecute()
        {
            return Application.Current is not null && Application.Current.MainWindow is not null;
        }

        public static void Execute()
        {
            Application.Current.MainWindow.Close();

            // If no need to handle closing event, use this:
            // Application.Current.Shutdown();
        }
    }
}
