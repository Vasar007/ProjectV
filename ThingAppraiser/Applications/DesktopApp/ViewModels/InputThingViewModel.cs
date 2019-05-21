using System.Collections.ObjectModel;
using System.Windows.Input;
using ThingAppraiser.DesktopApp.Domain.Commands;

namespace ThingAppraiser.DesktopApp.ViewModels
{
    internal class InputThingViewModel : ViewModelBase
    {
        private ObservableCollection<string> _thingList = new ObservableCollection<string>();

        private string _thingName;

        public ObservableCollection<string> ThingList
        {
            get => _thingList;
            set => SetProperty(ref _thingList, value);
        }

        public string ThingName
        {
            get => _thingName;
            set => SetProperty(ref _thingName, value);
        }

        public object DialogIdentifier { get; }

        public object DialogContent { get; set; }

        public ICommand EnterThingNameDialogCommand =>
            new RelayCommand(ExecutableDialogs.ExecuteEnterThingNameDialog);


        public InputThingViewModel(object dialogIdentifier, object dialogContent)
        {
            DialogIdentifier = dialogIdentifier.ThrowIfNull(nameof(dialogIdentifier));
            DialogContent = dialogContent.ThrowIfNull(nameof(dialogContent));
        }
    }
}
