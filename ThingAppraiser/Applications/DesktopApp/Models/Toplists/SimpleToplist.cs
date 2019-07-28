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

            Blocks.Add(block);

            return true;
        }

        public override bool RemoveBlock(ToplistBlock block)
        {
            block.ThrowIfNull(nameof(block));

            return Blocks.Remove(block);
        }

        #endregion
    }
}
