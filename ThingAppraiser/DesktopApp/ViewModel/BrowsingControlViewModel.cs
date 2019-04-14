using System.Collections.ObjectModel;
using System.Linq;
using DesktopApp.Model;
using DesktopApp.Model.DataSuppliers;
using ThingAppraiser;

namespace DesktopApp.ViewModel
{
    public class CBrowsingControlViewModel : CViewModelBase
    {
        private readonly IThingSupplier _thingSupplier;

        private CThing _selectedThing;

        public ObservableCollection<CThing> Things { get; set; }

        public CThing SelectedThing
        {
            get => _selectedThing;
            set => SetProperty(ref _selectedThing, _thingSupplier.GetThingByID(value.InternalID));
        }

        public CBrowsingControlViewModel(IThingSupplier thingSupplier)
        {
            _thingSupplier = thingSupplier.ThrowIfNull(nameof(thingSupplier));

            Things = new ObservableCollection<CThing>(_thingSupplier.GetAllThings());
            if (Things.Count > 0)
            {
                _selectedThing = _thingSupplier.GetThingByID(Things.First().InternalID);
            }
        }
    }
}
