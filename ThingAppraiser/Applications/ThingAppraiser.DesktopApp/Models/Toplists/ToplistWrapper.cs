using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using ThingAppraiser.Extensions;

namespace ThingAppraiser.DesktopApp.Models.Toplists
{
    internal sealed class ToplistWrapper
    {
        private readonly ToplistBase _toplist;

        public ObservableCollection<ToplistBlock> Blocks => _toplist.Blocks;


        public ToplistWrapper(ToplistBase toplist)
        {
            _toplist = toplist.ThrowIfNull(nameof(toplist));
        }

        public void AddNewBlock()
        {
            // Find first inconsistent block number in sorted sequence and select it as block number
            // to add.

            IReadOnlyList<int> existsNumbers = _toplist.Blocks
                .Select(block => block.Number)
                .OrderBy(number => number)
                .ToReadOnlyList();

            const int firstPosition = 1;

            int blockNumber = existsNumbers.Count > 0 && existsNumbers.First() != firstPosition
                ? firstPosition
                : _toplist.Blocks.Count + 1;

            if (blockNumber != firstPosition)
            {
                // This loop needs to check exists numbers and update result block to insert.
                for (int i = 1; i < existsNumbers.Count; ++i)
                {
                    if (existsNumbers[i] - existsNumbers[i - 1] != 1)
                    {
                        blockNumber = existsNumbers[i - 1] + 1;
                        break;
                    }
                }
            }

            _toplist.AddBlock(new ToplistBlock(blockNumber.ToString(), blockNumber));
        }

        public void RemoveBlock(ToplistBlock block)
        {
            block.ThrowIfNull(nameof(block));

            _toplist.RemoveBlock(block);
        }

        public void SaveToplist(string toplistFilename)
        {
            toplistFilename.ThrowIfNullOrEmpty(nameof(toplistFilename));

            string toplistData = ToplistBase.Serialize(_toplist);
            File.WriteAllText(toplistFilename, toplistData);
        }
    }
}
