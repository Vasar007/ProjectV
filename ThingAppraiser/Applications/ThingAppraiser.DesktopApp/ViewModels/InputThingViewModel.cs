using System.Collections.ObjectModel;
using Prism.Mvvm;
using ThingAppraiser.Extensions;

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


        public InputThingViewModel()
        {
            _thingList = new ObservableCollection<string>();
        }
    }
}
