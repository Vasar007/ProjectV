using System;
using System.Windows.Input;
using DesktopApp.Domain.Commands;
using ThingAppraiser;

namespace DesktopApp.ViewModel
{
    public class CStartControlViewModel : CViewModelBase
    {
        public Object DialogIdentifier { get; }

        public ICommand InputThingDialogCommand =>
            new CRelayCommand(SExecutableDialogs.ExecuteInputThingDialog);

        public ICommand OpenFileDialogCommand =>
            new CRelayCommand(SExecutableDialogs.ExecuteOpenFileDialog);

        public ICommand EnterDataDialogCommand =>
            new CRelayCommand(SExecutableDialogs.ExecuteEnterDataDialog);


        public CStartControlViewModel(Object dialogIdentifier)
        {
            DialogIdentifier = dialogIdentifier.ThrowIfNull(nameof(dialogIdentifier));
        }
    }
}
