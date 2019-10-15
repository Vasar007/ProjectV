using System.Linq;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using ThingAppraiser.DesktopApp.Domain;
using ThingAppraiser.DesktopApp.Views;
using ThingAppraiser.Extensions;
using ThingAppraiser.Logging;

namespace ThingAppraiser.DesktopApp.ViewModels
{
    internal static class ExecutableDialogs
    {
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor(typeof(ExecutableDialogs));


        public static string? ExecuteOpenThingsFileDialog()
        {
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
            var dialog = new VistaFolderBrowserDialog
            {
                Description = "Open content directory",
                UseDescriptionForTitle = true
            };

            bool? result = dialog.ShowDialog();
            return result.GetValueOrDefault() ? dialog.SelectedPath : null;
        }

        public static void ExecuteCreateToplistDialog(ToplistHeaderViewModel toplistStartViewModel)
        {
            toplistStartViewModel.ThrowIfNull(nameof(toplistStartViewModel));

            var view = new CreateToplistView();

            DialogHostProvider
                .ShowDialog(
                    view, toplistStartViewModel.DialogIdentifier,
                    CreateToplistClosingEventHandler
                )
                .FireAndForgetSafeAsync();
        }

        private static void CreateToplistClosingEventHandler(object sender,
            DialogClosingEventArgs eventArgs)
        {
            if (Equals(eventArgs.Parameter, false)) return;

            if (!(eventArgs.Parameter is MainWindowViewModel mainWindowViewModel)) return;
            if (!(eventArgs.Session.Content is CreateToplistView createToplistDialog)) return;
            if (!(createToplistDialog.DataContext is CreateToplistViewModel createToplistViewModel))
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(createToplistViewModel.ToplistName)) return;

            mainWindowViewModel.ConstructToplistEditor(
                createToplistViewModel.ToplistName,
                createToplistViewModel.SelectedToplistType,
                createToplistViewModel.SelectedToplistFormat
            );
        }
    }
}
