using System;
using System.Collections.Generic;
using Acolyte.Assertions;

namespace ProjectV.DesktopApp.Models.Toplists
{
    internal sealed class ScoreToplist : ToplistBase
    {
        public ScoreToplist(string name, ToplistFormat format)
            : base(name, ToplistType.Score, format)
        {
        }

        #region ToplistBase Overriden Methods

        public override bool AddBlock(ToplistBlock block)
        {
            block.ThrowIfNull(nameof(block));

            int insertIndex = CalculateInsertIndex(block.Number);
            Blocks.Insert(insertIndex, block);

            return true;
        }

        public override bool RemoveBlock(ToplistBlock block)
        {
            block.ThrowIfNull(nameof(block));

            return Blocks.Remove(block);
        }

        public override void UpdateBlocks(IEnumerable<ToplistBlock> blocks)
        {
            blocks.ThrowIfNull(nameof(blocks));

            base.UpdateBlocks(blocks);
        }

        #endregion

        private int CalculateInsertIndex(int blockNumber)
        {
            int insertIndex = Format switch
            {
                ToplistFormat.Forward => blockNumber - 1,

                ToplistFormat.Reverse => Blocks.Count - blockNumber + 1,

                _ => throw new InvalidOperationException(
                         $"Unknown toplist format: '{Format.ToString()}'."
                     )
            };

            // Additional checks to be sure that index is not out of range.
            if (insertIndex < 0)
            {
                insertIndex = 0;
            }
            if (insertIndex > Blocks.Count)
            {
                insertIndex = Blocks.Count;
            }

            return insertIndex;
        }
    }
}
