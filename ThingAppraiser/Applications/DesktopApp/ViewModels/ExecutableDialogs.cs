using System;
using System.Linq;
using System.Threading.Tasks;
using WinForms = System.Windows.Forms;
using MaterialDesignThemes.Wpf;
using ThingAppraiser.DesktopApp.Domain;
using ThingAppraiser.DesktopApp.Views;
using ThingAppraiser.Logging;
using ThingAppraiser.DesktopApp.Models.Toplists;

namespace ThingAppraiser.DesktopApp.ViewModels
{
    internal static class ExecutableDialogs
    {
        private static readonly LoggerAbstraction _logger =
            LoggerAbstraction.CreateLoggerInstanceWithName(nameof(ExecutableDialogs));


        public static void ExecuteInputThingDialog(StartViewModel startViewModel)
        {
            var view = new InputThingDialog();

            ShowDialog(view, startViewModel.DialogIdentifier, InputThingClosingEventHandler)
                .FireAndForgetSafeAsync(new CommonErrorHandler());
        }

        public static void ExecuteEnterThingNameDialog(InputThingViewModel inputThingViewModel)
        {
            inputThingViewModel.ThingName = string.Empty;

            ShowDialog(inputThingViewModel.DialogContent, inputThingViewModel.DialogIdentifier,
                       EnterThingNameClosingEventHandler)
                .FireAndForgetSafeAsync(new CommonErrorHandler());
        }

        public static void ExecuteOpenFileDialog(MainWindowViewModel mainViewModel)
        {
            using (var dialog = new WinForms.OpenFileDialog
            {
                DefaultExt = ".csv",
                Filter = "CSV Files (*.csv)|*.csv|Text Files (*.txt)|*.txt"
            })
            {

                WinForms.DialogResult result = dialog.ShowDialog();
                if (result == WinForms.DialogResult.OK)
                {
                    mainViewModel.SendRequestToService(DataSource.LocalFile, dialog.FileName);
                }
            }
        }

        public static void ExecuteEnterDataDialog(StartViewModel startViewModel)
        {
            var view = new EnterDataDialog(DesktopOptions.HintTexts.HintTextForGoogleDriveDialog);

            ShowDialogExtended(view, startViewModel.DialogIdentifier, EnterDataOpenedEventHandler,
                               EnterDataClosingEventHandler)
                .FireAndForgetSafeAsync(new CommonErrorHandler());
        }

        public static void ExecuteCreateToplistDialog(ToplistStartViewModel toplistStartViewModel)
        {
            var view = new CreateToplistDialog();

            ShowDialog(view, toplistStartViewModel.DialogIdentifier,
                       CreateToplistClosingEventHandler)
                .FireAndForgetSafeAsync(new CommonErrorHandler());
        }

        public static void ExecuteOpenToplistDialog(ToplistStartViewModel toplistStartViewModel)
        {
            var view = new OpenToplistDialog(toplistStartViewModel.DialogIdentifier);

            ShowDialog(view, toplistStartViewModel.DialogIdentifier, OpenToplistClosingEventHandler)
                .FireAndForgetSafeAsync(new CommonErrorHandler());
        }

        private static async Task ShowDialog(object content, object dialogIdentifier,
            DialogClosingEventHandler closingEventHandler)
        {
            var result = await DialogHost.Show(content, dialogIdentifier, closingEventHandler);

            _logger.Debug(
                $"Dialog was closed, the CommandParameter used to close it was: {result ?? "NULL"}"
            );
        }

        private static async Task ShowDialogExtended(object content, object dialogIdentifier,
            DialogOpenedEventHandler openedEventHandler,
            DialogClosingEventHandler closingEventHandler)
        {
            var result = await DialogHost.Show(content, dialogIdentifier, openedEventHandler,
                                               closingEventHandler);

            _logger.Debug(
                $"Dialog was closed, the CommandParameter used to close it was: {result ?? "NULL"}"
            );
        }

        private static void InputThingClosingEventHandler(object sender,
            DialogClosingEventArgs eventArgs)
        {
            if (Equals(eventArgs.Parameter, false)) return;

            if (!(eventArgs.Parameter is MainWindowViewModel mainWindowViewModel)) return;
            if (!(eventArgs.Session.Content is InputThingDialog inputThingDialog)) return;
            if (!(inputThingDialog.DataContext is InputThingViewModel inputThingViewModel)) return;

            if (inputThingViewModel.ThingList.IsNullOrEmpty()) return;

            mainWindowViewModel.SendRequestToService(
                DataSource.InputThing, inputThingViewModel.ThingList.ToList()
            );
        }

        private static void EnterThingNameClosingEventHandler(object sender,
            DialogClosingEventArgs eventArgs)
        {
            if (Equals(eventArgs.Parameter, false)) return;

            if (!(eventArgs.Parameter is InputThingViewModel inputThingViewModel)) return;

            if (string.IsNullOrWhiteSpace(inputThingViewModel.ThingName)) return;

            inputThingViewModel.ThingList.Add(inputThingViewModel.ThingName.Trim());
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
            if (!(eventArgs.Session.Content is EnterDataDialog enterDataDialog)) return;
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
            if (!(eventArgs.Session.Content is CreateToplistDialog createToplistDialog)) return;
            if (!(createToplistDialog.DataContext is CreateToplistViewModel createToplistViewModel))
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(createToplistViewModel.ToplistName)) return;

            mainWindowViewModel.OpenToplistEditorScene(
                createToplistViewModel.ToplistName,
                createToplistViewModel.SelectedToplistType,
                createToplistViewModel.SelectedToplistFormat
            );
        }

        private static void OpenToplistClosingEventHandler(object sender,
            DialogClosingEventArgs eventArgs)
        {
            if (Equals(eventArgs.Parameter, false)) return;

            if (!(eventArgs.Parameter is MainWindowViewModel _)) return;
            if (!(eventArgs.Session.Content is OpenToplistDialog openToplistDialog)) return;
            if (!(openToplistDialog.DataContext is OpenToplistViewModel _)) return;

            // TODO: add open toplist logic.
        }
    }
}
