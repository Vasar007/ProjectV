using System;
using System.Windows;

namespace DesktopApp.Domain.Commands
{
    public static class SApplicationCloseCommand
    {
        public static Boolean CanExecute(Object parameter)
        {
            return !(Application.Current is null) && !(Application.Current.MainWindow is null);
        }

        public static void Execute(Object parameter)
        {
            Application.Current.MainWindow.Close();
            // If no need to handle closing event, use this:
            // Application.Current.Shutdown();
        }
    }
}
