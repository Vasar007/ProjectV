using System.Collections.ObjectModel;

namespace ThingAppraiser.DesktopApp.Models.Toplists
{
    // TODO: implement add, remove and other methods to wotrk with items.
    internal class SimpleToplist : ToplistBase
    {
        public ObservableCollection<ToplistItem> Items { get; private set; }


        public SimpleToplist(string name, string type, string format)
            : base(name, type, format)
        {
            Items = new ObservableCollection<ToplistItem>();

            Items.Add(new ToplistItem("Name1", 1));
            Items.Add(new ToplistItem("Name2", 2));
            Items.Add(new ToplistItem("Name3", 3));
            Items.Add(new ToplistItem("Name4", 4));
            Items.Add(new ToplistItem("Name5", 5));
        }
    }
}
