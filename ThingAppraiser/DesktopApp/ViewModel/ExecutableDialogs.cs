using System;
using System.Linq;
using System.Threading.Tasks;
using WinForms = System.Windows.Forms;
using MaterialDesignThemes.Wpf;
using DesktopApp.Domain;
using DesktopApp.View;
using ThingAppraiser.Logging;

namespace DesktopApp.ViewModel
{
    public static class SExecutableDialogs
    {
        private static readonly CLoggerAbstraction s_logger =
            CLoggerAbstraction.CreateLoggerInstanceWithName(nameof(SExecutableDialogs));


        public static void ExecuteInputThingDialog(Object obj)
        {
            if (!(obj is CStartControlViewModel startControlViewModel)) return;

            var view = new CInputThingDialog();

            ShowDialog(view, startControlViewModel.DialogIdentifier, InputThingClosingEventHandler)
                .FireAndForgetSafeAsync(new CommonErrorHandler());
        }

        public static void ExecuteEnterThingNameDialog(Object obj)
        {
            if (!(obj is CInputThingViewModel inputDatViewModel)) return;

            inputDatViewModel.ThingName = String.Empty;

            ShowDialog(inputDatViewModel.DialogContent, inputDatViewModel.DialogIdentifier,
                       EnterThingNameClosingEventHandler)
                .FireAndForgetSafeAsync(new CommonErrorHandler());
        }

        public static void ExecuteOpenFileDialog(Object obj)
        {
            var dialog = new WinForms.OpenFileDialog
            {
                DefaultExt = ".txt",
                Filter = @"Text Files (*.txt)|*.txt|CSV Files (*.csv)|*.csv"
            };

            WinForms.DialogResult result = dialog.ShowDialog();
            if (result == WinForms.DialogResult.OK && obj is CMainWindowViewModel viewModel)
            {
                viewModel.SetDataSourceAndParameters(EDataSource.LocalFile, dialog.FileName);
            }
        }

        public static void ExecuteEnterDataDialog(Object obj)
        {
            if (!(obj is CStartControlViewModel startControlViewModel)) return;

            var view = new CEnterDataDialog();

            ShowDialogExtended(view, startControlViewModel.DialogIdentifier,
                               EnterDataOpenedEventHandler, EnterDataClosingEventHandler)
                .FireAndForgetSafeAsync(new CommonErrorHandler());
        }

        private static async Task ShowDialog(Object content, Object dialogIdentifier,
            DialogClosingEventHandler closingEventHandler)
        {
            var result = await DialogHost.Show(content, dialogIdentifier, closingEventHandler);

            s_logger.Debug(
                $"Dialog was closed, the CommandParameter used to close it was: {result ?? "NULL"}"
            );
        }

        private static async Task ShowDialogExtended(Object content, Object dialogIdentifier,
            DialogOpenedEventHandler openedEventHandler,
            DialogClosingEventHandler closingEventHandler)
        {
            var result = await DialogHost.Show(content, dialogIdentifier, openedEventHandler,
                                               closingEventHandler);

            s_logger.Debug(
                $"Dialog was closed, the CommandParameter used to close it was: {result ?? "NULL"}"
            );
        }

        private static void InputThingClosingEventHandler(Object sender,
            DialogClosingEventArgs eventArgs)
        {
            if (Equals(eventArgs.Parameter, false)) return;

            if (!(eventArgs.Parameter is CMainWindowViewModel mainWindowViewModel)) return;
            if (!(eventArgs.Session.Content is CInputThingDialog inputThingDialog)) return;
            if (!(inputThingDialog.DataContext is CInputThingViewModel inputDatViewModel)) return;
            if (inputDatViewModel.ThingList.Count == 0) return;

            eventArgs.Cancel();
            eventArgs.Session.UpdateContent(new CProgressDialog
            {
                DataContext = new CProgressDialogViewModel(32)
            });

            mainWindowViewModel.SetDataSourceAndParameters(
                EDataSource.InputThing,
                inputDatViewModel.ThingList.ToList()
            );
        }

        private static void EnterThingNameClosingEventHandler(Object sender,
            DialogClosingEventArgs eventArgs)
        {
            if (Equals(eventArgs.Parameter, false)) return;

            if (!(eventArgs.Parameter is CInputThingViewModel inputDatViewModel)) return;

            if (!String.IsNullOrWhiteSpace(inputDatViewModel.ThingName))
            {
                inputDatViewModel.ThingList.Add(inputDatViewModel.ThingName.Trim());
            }
        }

        private static void EnterDataOpenedEventHandler(Object sender,
            DialogOpenedEventArgs eventArgs)
        {
            s_logger.Debug("Dialog was opened.");
        }

        private static void EnterDataClosingEventHandler(Object sender,
            DialogClosingEventArgs eventArgs)
        {
            if (Equals(eventArgs.Parameter, false)) return;

            if (!(eventArgs.Parameter is CMainWindowViewModel mainWindowViewModel)) return;
            if (!(eventArgs.Session.Content is CEnterDataDialog enterDataDialog)) return;
            if (!(enterDataDialog.DataContext is CEnterDataDialogViewModel enterDataViewModel))
            {
                return;
            }

            eventArgs.Cancel();
            eventArgs.Session.UpdateContent(new CProgressDialog
            {
                DataContext = new CProgressDialogViewModel(32)
            });

            mainWindowViewModel.SetDataSourceAndParameters(EDataSource.GoogleDrive,
                                                           enterDataViewModel.Name);
        }
    }
}
