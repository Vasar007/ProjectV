using System.Collections.ObjectModel;
using Acolyte.Assertions;
using Prism.Mvvm;

namespace ThingAppraiser.DesktopApp.ViewModels
{
    internal sealed class InputThingViewModel : BindableBase
    {
        private ObservableCollection<string> _thingList;
        public ObservableCollection<string> ThingList
        {
            get => _thingList;
            set => SetProperty(ref _thingList, value.ThrowIfNull(nameof(value)));
        }

        private string _thingName;
        public string ThingName
        {
            get => _thingName;
            set => SetProperty(ref _thingName, value.ThrowIfNull(nameof(value)));
        }

        public InputThingViewModel()
        {
            _thingList = new ObservableCollection<string>();
            _thingName = string.Empty;
        }
    }
}
