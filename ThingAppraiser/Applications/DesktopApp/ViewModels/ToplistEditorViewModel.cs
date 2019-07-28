using System.Collections.ObjectModel;
using System.Windows.Input;
using ThingAppraiser.DesktopApp.Domain.Commands;
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

        public ICommand AddOrUpdateCommand => new RelayCommand(AddNewBlock);


        public ToplistEditorViewModel()
        {
        }

        public void ConstructNewToplist(string toplistName, ToplistType toplistType,
            ToplistFormat toplistFormat)
        {
            _toplist = ToplistFactory.Create(toplistName, toplistType, toplistFormat);
            ToplistBlocks = _toplist.Blocks;

            AddNewBlock();
        }

        public void UpdateToplist()
        {
            // TODO: add/remove elements from toplist items.
        }

        private void AddNewBlock()
        {
            int blockNumber = _toplist.Blocks.Count + 1;
            _toplist.AddBlock(new ToplistBlock(blockNumber.ToString(), blockNumber));
        }
    }
}
