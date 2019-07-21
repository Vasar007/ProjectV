using System.Collections.ObjectModel;
using ThingAppraiser.DesktopApp.Views;

namespace ThingAppraiser.DesktopApp.Models.Toplists
{
    internal class ToplistBlock : ModelBase
    {
        private string _title;

        private ToplistItem _creationToplistItem;

        private ToplistBox _creationToplistBox;

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value.ThrowIfNull(nameof(value)));
        }

        public ObservableCollection<ToplistItem> Items { get; }
           = new ObservableCollection<ToplistItem>();

        public ToplistBox CreationToplistBox
        {
            get => _creationToplistBox;
            set => SetProperty(ref _creationToplistBox, value.ThrowIfNull(nameof(value)));
        }

        public ToplistItem CreationToplistItem
        {
            get => _creationToplistItem;
            set => SetProperty(ref _creationToplistItem, value.ThrowIfNull(nameof(value)));
        }


        public ToplistBlock(string title)
        {
            Title = title;

            _creationToplistBox = new ToplistBox(new ToplistItem(null, string.Empty, this));
        }

        public bool AddOrUpdateItem(ToplistItem toplistItem)
        {
            toplistItem.ThrowIfNull(nameof(toplistItem));

            bool nameIsNullOrWhitespace = string.IsNullOrWhiteSpace(toplistItem.Name);

            if (Items.Contains(toplistItem))
            {
                if (nameIsNullOrWhitespace)
                {
                    RemoveItem(toplistItem);
                }

                return false;
            }

            if (nameIsNullOrWhitespace) return false;

            Items.Add(toplistItem.Clone());
            return true;
        }

        public bool RemoveItem(ToplistItem toplistItem)
        {
            toplistItem.ThrowIfNull(nameof(toplistItem));

            return Items.Remove(toplistItem);
        }

        public bool DeleteItemIfNeed(ToplistItem toplistItem)
        {
            toplistItem.ThrowIfNull(nameof(toplistItem));

            // TODO: refactor this.
            if (ReferenceEquals(CreationToplistItem, toplistItem)) return false;

            if (Items.Contains(toplistItem) &&
                string.IsNullOrWhiteSpace(toplistItem.Name))
            {
                RemoveItem(toplistItem);
                return true;
            }

            return false;
        }
    }
}
