using System.Collections.ObjectModel;
using ThingAppraiser.DesktopApp.Models.Toplists;

namespace ThingAppraiser.DesktopApp.ViewModels
{
    internal class ToplistEditorViewModel : ViewModelBase
    {
        private Toplist _toplist;

        private ObservableCollection<ToplistItem> _toplistItems =
            new ObservableCollection<ToplistItem>();

        public ObservableCollection<ToplistItem> ToplistItems
        {
            get => _toplistItems;
            set => SetProperty(ref _toplistItems, value.ThrowIfNull(nameof(value)));
        }


        public ToplistEditorViewModel()
        {
            //_toplist = new Toplist("Default", "Default", "Default");
        }

        public void ConstructNewToplist(string toplistName, string toplistType,
            string toplistFormat)
        {
            _toplist = new Toplist(toplistName, toplistType, toplistFormat);
            ToplistItems = _toplist.Items;
        }

        public void UpdateToplist()
        {
            // TODO: add/remove elements from toplist items.
        }
    }
}
