using System.Collections.Generic;

namespace ThingAppraiser.DesktopApp.Models.Toplists
{
    // TODO: implement add, remove and other methods to work with items.
    internal class ScoreToplist : ToplistBase
    {
        private readonly Dictionary<int, ToplistBlock> _blocks =
            new Dictionary<int, ToplistBlock>();


        public ScoreToplist(string name, string type, string format)
            : base(name, type, format)
        {
            FillData();
        }

        private void FillData()
        {
            var block = new ToplistBlock("Block1");

            Blocks.Add(block);
            _blocks.Add(1, block);

            block = new ToplistBlock("Block2");

            Blocks.Add(block);
            _blocks.Add(2, block);

            block = new ToplistBlock("Block3");

            Blocks.Add(block);
            _blocks.Add(3, block);
        }
    }
}
