namespace ThingAppraiser.DesktopApp.Models.Toplists
{
    // TODO: implement add, remove and other methods to work with items.
    internal class SimpleToplist : ToplistBase
    {
        public SimpleToplist(string name, string type, string format)
            : base(name, type, format)
        {
            FillData();
        }

        private void FillData()
        {
            Blocks.Add(new ToplistBlock("Block1"));
            Blocks.Add(new ToplistBlock("Block2"));
            Blocks.Add(new ToplistBlock("Block3"));
        }
    }
}
