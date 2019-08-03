using System.Linq;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ThingAppraiser.DesktopApp.Domain.Commands;
using ThingAppraiser.DesktopApp.Models.Toplists;
using System.Collections.Generic;
using System.IO;

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

        public ICommand AddOrUpdateBlockCommand => new RelayCommand(AddNewBlock);

        public ICommand RemoveBlockCommand => new RelayCommand<ToplistBlock>(RemoveBlock);

        public ICommand SaveToplistCommand => new RelayCommand(SaveToplist);


        public ToplistEditorViewModel()
        {
        }

        public void ConstructNewToplist(string toplistName, ToplistType toplistType,
            ToplistFormat toplistFormat)
        {
            toplistName.ThrowIfNullOrEmpty(nameof(toplistName));

            _toplist = ToplistFactory.Create(toplistName, toplistType, toplistFormat);
            ToplistBlocks = _toplist.Blocks;

            AddNewBlock();
        }

        public void LoadToplist(string toplistFilename)
        {
            toplistFilename.ThrowIfNullOrEmpty(nameof(toplistFilename));

            _toplist = ToplistFactory.LoadFromFile(toplistFilename);
            ToplistBlocks = _toplist.Blocks;
        }

        private void AddNewBlock()
        {
            if (_toplist is null) return;

            // Find first inconsistent block number in sorted sequence and select it as block number
            // to add.

            IReadOnlyList<int> existsNumbers = _toplist.Blocks
                .Select(block => block.Number)
                .OrderBy(number => number)
                .ToList();

            int blockNumber = existsNumbers.Count > 0 && existsNumbers.First() != 1
                ? 1
                : _toplist.Blocks.Count + 1;

            for (int i = 1; i < existsNumbers.Count; ++i)
            {
                if (existsNumbers[i] - existsNumbers[i - 1] != 1)
                {
                    blockNumber = existsNumbers[i - 1] + 1;
                    break;
                }
            }

            _toplist.AddBlock(new ToplistBlock(blockNumber.ToString(), blockNumber));
        }

        private void RemoveBlock(ToplistBlock block)
        {
            block.ThrowIfNull(nameof(block));

            _toplist.RemoveBlock(block);
        }

        private void SaveToplist()
        {
            string toplistData = ToplistBase.Serialize(_toplist);

            // TODO: implement saving logic to specified path by user.
            File.WriteAllText("Toplist.txt", toplistData);
        }
    }
}
