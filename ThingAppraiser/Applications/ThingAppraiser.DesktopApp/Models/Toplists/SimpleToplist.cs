using System;

namespace ThingAppraiser.DesktopApp.Models.Toplists
{
    internal sealed class SimpleToplist : ToplistBase
    {
        public SimpleToplist(string name, ToplistFormat format)
            : base(name, ToplistType.Simple, format)
        {
        }

        #region ToplistBase Overriden Methods

        public override bool AddBlock(ToplistBlock block)
        {
            block.ThrowIfNull(nameof(block));

            int insertIndex = CalculateInsertIndex();
            Blocks.Insert(insertIndex, block);

            return true;
        }

        public override bool RemoveBlock(ToplistBlock block)
        {
            block.ThrowIfNull(nameof(block));

            return Blocks.Remove(block);
        }

        #endregion

        private int CalculateInsertIndex()
        {
            int insertIndex = Format switch
            {
                ToplistFormat.Forward => Blocks.Count,

                ToplistFormat.Reverse => 0,

                _ => throw new InvalidOperationException(
                         $"Unknown toplist format: '{Format.ToString()}'."
                     )
            };

            return insertIndex;
        }
    }
}
