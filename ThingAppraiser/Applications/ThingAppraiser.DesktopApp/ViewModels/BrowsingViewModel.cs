using System.Collections.ObjectModel;
using System.Linq;
using ThingAppraiser.Extensions;
using ThingAppraiser.DesktopApp.Models.DataSuppliers;
using ThingAppraiser.DesktopApp.Models.Things;
using ThingAppraiser.Models.WebService;
using Prism.Mvvm;

namespace ThingAppraiser.DesktopApp.ViewModels
{
    internal sealed class BrowsingViewModel : BindableBase
    {
        private readonly IThingSupplier _thingSupplier;

        public ObservableCollection<Thing> Things { get; private set; }

        private Thing? _selectedThing;
        public Thing? SelectedThing
        {
            get => _selectedThing;
            set
            {
                if (value is null)
                {
                    SetProperty(ref _selectedThing, null);
                    return;
                }
                SetProperty(ref _selectedThing, _thingSupplier.GetThingById(value.InternalId));
            }
        }

        public BrowsingViewModel(IThingSupplier thingSupplier)
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

        public void Update(ProcessingResponse response)
        {
            response.ThrowIfNull(nameof(response));

            _thingSupplier.SaveResults(response, "Service response");

            Update();
        }
    }
}
