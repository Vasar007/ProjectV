using System.Windows.Input;
using ThingAppraiser.DesktopApp.Domain.Commands;

namespace ThingAppraiser.DesktopApp.ViewModels
{
    internal class StartControlViewModel : ViewModelBase
    {
        public object DialogIdentifier { get; }

        public ICommand InputThingDialogCommand =>
            new RelayCommand(ExecutableDialogs.ExecuteInputThingDialog);

        public ICommand OpenFileDialogCommand =>
            new RelayCommand(ExecutableDialogs.ExecuteOpenFileDialog);

        public ICommand EnterDataDialogCommand =>
            new RelayCommand(ExecutableDialogs.ExecuteEnterDataDialog);


        public StartControlViewModel(object dialogIdentifier)
        {
            DialogIdentifier = dialogIdentifier.ThrowIfNull(nameof(dialogIdentifier));
        }
    }
}
