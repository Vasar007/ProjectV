using System.Collections.ObjectModel;
using System.Windows.Input;
using ThingAppraiser.DesktopApp.Domain.Commands;

namespace ThingAppraiser.DesktopApp.ViewModels
{
    internal sealed class InputThingViewModel : ViewModelBase
    {
        private ObservableCollection<string> _thingList = new ObservableCollection<string>();

        private string _thingName = string.Empty;

        public ObservableCollection<string> ThingList
        {
            get => _thingList;
            set => SetProperty(ref _thingList, value.ThrowIfNull(nameof(value)));
        }

        public string ThingName
        {
            get => _thingName;
            set => SetProperty(ref _thingName, value.ThrowIfNull(nameof(value)));
        }

        public object DialogIdentifier { get; }

        public object DialogContent { get; }

        public ICommand EnterThingNameDialogCommand =>
            new RelayCommand<InputThingViewModel>(ExecutableDialogs.ExecuteEnterThingNameDialog);


        public InputThingViewModel(object dialogIdentifier, object dialogContent)
        {
            DialogIdentifier = dialogIdentifier.ThrowIfNull(nameof(dialogIdentifier));
            DialogContent = dialogContent.ThrowIfNull(nameof(dialogContent));
        }
    }
}
