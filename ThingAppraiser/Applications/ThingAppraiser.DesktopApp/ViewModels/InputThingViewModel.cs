using System.Collections.ObjectModel;
using System.Windows.Input;
using Prism.Mvvm;
using ThingAppraiser.DesktopApp.Domain.Commands;
using ThingAppraiser.Extensions;

namespace ThingAppraiser.DesktopApp.ViewModels
{
    internal sealed class InputThingViewModel : BindableBase
    {
        private ObservableCollection<string> _thingList = new ObservableCollection<string>();
        public ObservableCollection<string> ThingList
        {
            get => _thingList;
            set => SetProperty(ref _thingList, value.ThrowIfNull(nameof(value)));
        }

        private string _thingName = string.Empty;
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
