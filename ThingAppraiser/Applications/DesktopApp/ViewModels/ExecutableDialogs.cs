using System.Linq;
using System.Threading.Tasks;
using WinForms = System.Windows.Forms;
using MaterialDesignThemes.Wpf;
using ThingAppraiser.DesktopApp.Domain;
using ThingAppraiser.DesktopApp.Views;
using ThingAppraiser.Logging;

namespace ThingAppraiser.DesktopApp.ViewModels
{
    public static class ExecutableDialogs
    {
        private static readonly LoggerAbstraction _logger =
            LoggerAbstraction.CreateLoggerInstanceWithName(nameof(ExecutableDialogs));


        public static void ExecuteInputThingDialog(object obj)
        {
            if (!(obj is StartControlViewModel startControlViewModel)) return;

            var view = new InputThingDialog();

            ShowDialog(view, startControlViewModel.DialogIdentifier, InputThingClosingEventHandler)
                .FireAndForgetSafeAsync(new CommonErrorHandler());
        }

        public static void ExecuteEnterThingNameDialog(object obj)
        {
            if (!(obj is InputThingViewModel inputDatViewModel)) return;

            inputDatViewModel.ThingName = string.Empty;

            ShowDialog(inputDatViewModel.DialogContent, inputDatViewModel.DialogIdentifier,
                       EnterThingNameClosingEventHandler)
                .FireAndForgetSafeAsync(new CommonErrorHandler());
        }

        public static void ExecuteOpenFileDialog(object obj)
        {
            var dialog = new WinForms.OpenFileDialog
            {
                DefaultExt = ".csv",
                Filter = @"CSV Files (*.csv)|*.csv|Text Files (*.txt)|*.txt"
            };

            WinForms.DialogResult result = dialog.ShowDialog();
            if (result == WinForms.DialogResult.OK && obj is MainWindowViewModel viewModel)
            {
                viewModel.SetDataSourceAndParameters(DataSource.LocalFile, dialog.FileName);
            }
        }

        public static void ExecuteEnterDataDialog(object obj)
        {
            if (!(obj is StartControlViewModel startControlViewModel)) return;

            var view = new EnterDataDialog();

            ShowDialogExtended(view, startControlViewModel.DialogIdentifier,
                               EnterDataOpenedEventHandler, EnterDataClosingEventHandler)
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
            if (!(inputThingDialog.DataContext is InputThingViewModel inputDatViewModel)) return;
            if (inputDatViewModel.ThingList.IsNullOrEmpty()) return;

            mainWindowViewModel.SetDataSourceAndParameters(
                DataSource.InputThing,
                inputDatViewModel.ThingList.ToList()
            );
        }

        private static void EnterThingNameClosingEventHandler(object sender,
            DialogClosingEventArgs eventArgs)
        {
            if (Equals(eventArgs.Parameter, false)) return;

            if (!(eventArgs.Parameter is InputThingViewModel inputDatViewModel)) return;

            if (!string.IsNullOrWhiteSpace(inputDatViewModel.ThingName))
            {
                inputDatViewModel.ThingList.Add(inputDatViewModel.ThingName.Trim());
            }
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
            if (!(enterDataDialog.DataContext is EnterDataDialogViewModel enterDataViewModel))
            {
                return;
            }
            if (string.IsNullOrWhiteSpace(enterDataViewModel.Name)) return;

            mainWindowViewModel.SetDataSourceAndParameters(
                DataSource.GoogleDrive,
                enterDataViewModel.Name
            );
        }
    }
}
