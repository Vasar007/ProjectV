using System.Collections.ObjectModel;
using System.Linq;
using ThingAppraiser.DesktopApp.Models;
using ThingAppraiser.DesktopApp.Models.DataSuppliers;

namespace ThingAppraiser.DesktopApp.ViewModels
{
    public class BrowsingControlViewModel : ViewModelBase
    {
        private readonly IThingSupplier _thingSupplier;

        private Thing _selectedThing;

        public ObservableCollection<Thing> Things { get; set; }

        public Thing SelectedThing
        {
            get => _selectedThing;
            set => SetProperty(ref _selectedThing, _thingSupplier.GetThingById(value.InternalId));
        }

        public BrowsingControlViewModel(IThingSupplier thingSupplier)
        {
            _thingSupplier = thingSupplier.ThrowIfNull(nameof(thingSupplier));

            Things = new ObservableCollection<Thing>(_thingSupplier.GetAllThings());
            if (Things.Count > 0)
            {
                _selectedThing = _thingSupplier.GetThingById(Things.First().InternalId);
            }
        }
    }
}
