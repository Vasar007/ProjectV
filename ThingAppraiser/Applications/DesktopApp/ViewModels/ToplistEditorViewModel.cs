using System.Collections.ObjectModel;
using ThingAppraiser.DesktopApp.Models.Toplists;

namespace ThingAppraiser.DesktopApp.ViewModels
{
    // TODO: create additional placeholder for toplist items, when we use simple toplist it should
    // be casual panel for one toplist item but when we use score toplist it should be panel with
    // text box to keep toplist item info (place, number of items on panel) and list to keep toplist
    // items with equal key (i.e. all items with same place in toplist).
    internal class ToplistEditorViewModel : ViewModelBase
    {
        private SimpleToplist _toplist;

        private ObservableCollection<ToplistItem> _toplistItems =
            new ObservableCollection<ToplistItem>();

        public ObservableCollection<ToplistItem> ToplistItems
        {
            get => _toplistItems;
            set => SetProperty(ref _toplistItems, value.ThrowIfNull(nameof(value)));
        }


        public ToplistEditorViewModel()
        {
            // May be need to initialize toplist here.
        }

        public void ConstructNewToplist(string toplistName, string toplistType,
            string toplistFormat)
        {
            _toplist = new SimpleToplist(toplistName, toplistType, toplistFormat);
            ToplistItems = _toplist.Items;
        }

        public void UpdateToplist()
        {
            // TODO: add/remove elements from toplist items.
        }
    }
}
