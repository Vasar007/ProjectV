using System.Windows.Input;
using ThingAppraiser.DesktopApp.Domain.Commands;
using ThingAppraiser.Extensions;

namespace ThingAppraiser.DesktopApp.ViewModels
{
    internal sealed class ToplistStartViewModel : ViewModelBase
    {
        public object DialogIdentifier { get; }

        public ICommand CreateToplistDialogCommand =>
            new RelayCommand<ToplistStartViewModel>(ExecutableDialogs.ExecuteCreateToplistDialog);

        public ICommand OpenToplistDialogCommand =>
            new RelayCommand<ToplistStartViewModel>(ExecutableDialogs.ExecuteOpenToplistDialog);


        public ToplistStartViewModel(object dialogIdentifier)
        {
            DialogIdentifier = dialogIdentifier.ThrowIfNull(nameof(dialogIdentifier));
        }
    }
}
