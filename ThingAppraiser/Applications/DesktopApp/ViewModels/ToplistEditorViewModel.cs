using System.Collections.ObjectModel;
using ThingAppraiser.DesktopApp.Models.Toplists;

namespace ThingAppraiser.DesktopApp.ViewModels
{
    internal class ToplistEditorViewModel : ViewModelBase
    {
        private ToplistBase _toplist;

        private ObservableCollection<ToplistBlock> _toplistBlocks =
            new ObservableCollection<ToplistBlock>();

        public ObservableCollection<ToplistBlock> ToplistBlocks
        {
            get => _toplistBlocks;
            set => SetProperty(ref _toplistBlocks, value.ThrowIfNull(nameof(value)));
        }

        public ToplistEditorViewModel()
        {
            // May be need to initialize toplist blocks here.
        }

        public void ConstructNewToplist(string toplistName, string toplistType,
            string toplistFormat)
        {
            _toplist = new SimpleToplist(toplistName, toplistType, toplistFormat);
            ToplistBlocks = _toplist.Blocks;
        }

        public void UpdateToplist()
        {
            // TODO: add/remove elements from toplist items.
        }
    }
}
