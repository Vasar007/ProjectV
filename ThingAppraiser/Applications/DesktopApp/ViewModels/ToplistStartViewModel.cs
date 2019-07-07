using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using ThingAppraiser.DesktopApp.Domain.Commands;

namespace ThingAppraiser.DesktopApp.ViewModels
{
    internal class ToplistStartViewModel : ViewModelBase
    {
        public object DialogIdentifier { get; }

        public ICommand CreateToplistDialogCommand =>
            new RelayCommand(ExecutableDialogs.ExecuteCreateToplistDialog);

        public ICommand OpenToplistDialogCommand =>
            new RelayCommand(ExecutableDialogs.ExecuteOpenToplistDialog);


        public ToplistStartViewModel(object dialogIdentifier)
        {
            DialogIdentifier = dialogIdentifier.ThrowIfNull(nameof(dialogIdentifier));
        }
    }
}
