using System.Collections.ObjectModel;
using System.Linq;
using ThingAppraiser.DesktopApp.Models;
using ThingAppraiser.DesktopApp.Models.DataSuppliers;

namespace ThingAppraiser.DesktopApp.ViewModels
{
    internal class BrowsingControlViewModel : ViewModelBase
    {
        private readonly IThingSupplier _thingSupplier;

        private Thing _selectedThing;

        public ObservableCollection<Thing> Things { get; private set; }

        public Thing SelectedThing
        {
            get => _selectedThing;
            set => SetProperty(ref _selectedThing, _thingSupplier.GetThingById(value.InternalId));
        }

        public BrowsingControlViewModel(IThingSupplier thingSupplier)
        {
            _thingSupplier = thingSupplier.ThrowIfNull(nameof(thingSupplier));

            Things = new ObservableCollection<Thing>();
            Update();
        }

        public void Update()
        {
            if (Things.Count > 0)
            {
                Things.Clear();
            }
            foreach (Thing thing in _thingSupplier.GetAllThings())
            {
                Things.Add(thing);
            }

            if (Things.Count > 0)
            {
                SelectedThing = _thingSupplier.GetThingById(Things.First().InternalId);
            }
        }
    }
}
