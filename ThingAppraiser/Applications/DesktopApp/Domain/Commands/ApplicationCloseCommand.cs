using System.Windows;

namespace ThingAppraiser.DesktopApp.Domain.Commands
{
    public static class ApplicationCloseCommand
    {
        public static bool CanExecute(object _)
        {
            return !(Application.Current is null) && !(Application.Current.MainWindow is null);
        }

        public static void Execute(object _)
        {
            Application.Current.MainWindow.Close();

            // If no need to handle closing event, use this:
            // Application.Current.Shutdown();
        }
    }
}
