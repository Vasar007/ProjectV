using ThingAppraiser.DesktopApp.Domain;

namespace ThingAppraiser.DesktopApp.Models.Toplists
{
    // TODO: implement add, remove and other methods to wotrk with items.
    internal class ScoreToplist : ToplistBase
    {
        public ObservableDictionary<int, ToplistItem> Items { get; private set; }


        public ScoreToplist(string name, string type, string format)
            : base(name, type, format)
        {
            Items = new ObservableDictionary<int, ToplistItem>();

            Items.Add(1, new ToplistItem("Name1", 1));
            Items.Add(2, new ToplistItem("Name2", 2));
            Items.Add(3, new ToplistItem("Name3", 3));
            Items.Add(4, new ToplistItem("Name4", 4));
            Items.Add(5, new ToplistItem("Name5", 5));
        }
    }
}
