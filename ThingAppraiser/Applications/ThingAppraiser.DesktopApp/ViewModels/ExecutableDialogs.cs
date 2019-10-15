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

        public static void ExecuteInputThingDialog(StartViewModel startViewModel)
        {
            startViewModel.ThrowIfNull(nameof(startViewModel));

            var view = new InputThingView();

            DialogHostProvider
                .ShowDialog(
                    view, startViewModel.DialogIdentifier,
                    InputThingClosingEventHandler
                )
                .FireAndForgetSafeAsync();
        }

        public static void ExecuteEnterDataDialog(StartViewModel startViewModel)
        {
            startViewModel.ThrowIfNull(nameof(startViewModel));

            var view = new EnterDataView(DesktopOptions.HintTexts.HintTextForGoogleDriveDialog);

            DialogHostProvider
                .ShowDialogExtended(
                    view, startViewModel.DialogIdentifier,
                    EnterDataOpenedEventHandler, EnterDataClosingEventHandler
                )
                .FireAndForgetSafeAsync();
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

        private static void InputThingClosingEventHandler(object sender,
            DialogClosingEventArgs eventArgs)
        {
            if (Equals(eventArgs.Parameter, false)) return;

            if (!(eventArgs.Parameter is MainWindowViewModel mainWindowViewModel)) return;
            if (!(eventArgs.Session.Content is InputThingView inputThingDialog)) return;
            if (!(inputThingDialog.DataContext is InputThingViewModel inputThingViewModel)) return;

            if (inputThingViewModel.ThingList.IsNullOrEmpty()) return;

            mainWindowViewModel.SendRequestToService(
                DataSource.InputThing, inputThingViewModel.ThingList.ToList()
            );
        }

        private static void EnterDataOpenedEventHandler(object sender,
            DialogOpenedEventArgs eventArgs)
        {
            _logger.Debug("Dialog was opened.");
        }

        private static void EnterDataClosingEventHandler(object sender,
            DialogClosingEventArgs eventArgs)
        {
            if (Equals(eventArgs.Parameter, false)) return;

            if (!(eventArgs.Parameter is MainWindowViewModel mainWindowViewModel)) return;
            if (!(eventArgs.Session.Content is EnterDataView enterDataDialog)) return;
            if (!(enterDataDialog.DataContext is EnterDataViewModel enterDataViewModel)) return;

            if (string.IsNullOrWhiteSpace(enterDataViewModel.Name)) return;

            mainWindowViewModel.SendRequestToService(
                DataSource.GoogleDrive, enterDataViewModel.Name
            );
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
