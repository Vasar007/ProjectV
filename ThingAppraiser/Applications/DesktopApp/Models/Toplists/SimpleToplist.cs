﻿using System;

namespace ThingAppraiser.DesktopApp.Models.Toplists
{
    internal class SimpleToplist : ToplistBase
    {
        public SimpleToplist(string name, ToplistFormat format)
            : base(name, ToplistType.Simple, format)
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

        #endregion

        private int CalculateInsertIndex(int blockNumber)
        {
            int insertIndex;
            switch (Format)
            {
                case ToplistFormat.Forward:
                {
                    insertIndex = Blocks.Count;
                    break;
                }

                case ToplistFormat.Reverse:
                {
                    insertIndex = 0;
                    break;
                }

                default:
                {
                    throw new Exception($"Unknown toplist format: '{Format.ToString()}'.");
                }
            }

            return insertIndex;
        }
    }
}
