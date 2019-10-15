using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using ThingAppraiser.Logging;

namespace ThingAppraiser.DesktopApp.ViewModels
{
    // TODO: may be need to create methods with common logic and provide the remaining methods as
    // wrappers (they should specify parameters for dialogs).
    internal static class ExecutableDialogs
    {
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor(typeof(ExecutableDialogs));


        public static string? ExecuteOpenThingsFileDialog()
        {
            _logger.Info("Executing open things file dialog.");

            var dialog = new OpenFileDialog
            {
                Title = "Open Things file",
                FileName = "things.txt",
                DefaultExt = ".csv",
                Filter = "CSV Files (*.csv)|*.csv|Text Files (*.txt)|*.txt",
                ValidateNames = true,
                CheckFileExists = true
            };

            bool? result = dialog.ShowDialog();
            return result.GetValueOrDefault() ? dialog.FileName : null;
        }

        public static string? ExecuteOpenToplistFileDialog()
        {
            _logger.Info("Executing open toplist file dialog.");

            var dialog = new OpenFileDialog
            {
                Title = "Open toplist file",
                FileName = "toplist.txt",
                DefaultExt = ".txt",
                Filter = "Text Files (*.txt)|*.txt",
                ValidateNames = true,
                CheckFileExists = true
            };

            bool? result = dialog.ShowDialog();
            return result.GetValueOrDefault() ? dialog.FileName : null;
        }

        public static string? ExecuteSaveToplistFileDialog()
        {
            _logger.Info("Executing save toplist file dialog.");

            var dialog = new SaveFileDialog
            {
                Title = "Save toplist file",
                FileName = "toplist.txt",
                DefaultExt = ".txt",
                Filter = "Text Files (*.txt)|*.txt",
                ValidateNames = true
            };

            bool? result = dialog.ShowDialog();
            return result.GetValueOrDefault() ? dialog.FileName : null;
        }

        public static string? ExecuteOpenContentDirectoryDialog()
        {
            _logger.Info("Executing open content directory dialog.");

            var dialog = new VistaFolderBrowserDialog
            {
                Description = "Open content directory",
                UseDescriptionForTitle = true
            };

            bool? result = dialog.ShowDialog();
            return result.GetValueOrDefault() ? dialog.SelectedPath : null;
        }
    }
}
