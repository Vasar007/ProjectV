using System.Collections.ObjectModel;
using ThingAppraiser.DesktopApp.Views;

namespace ThingAppraiser.DesktopApp.Models.Toplists
{
    internal class ToplistBlock : ModelBase
    {
        private string _title;

        private ToplistBox _creationToplistBox;

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value.ThrowIfNull(nameof(value)));
        }

        public ToplistBox CreationToplistBox
        {
            get => _creationToplistBox;
            set => SetProperty(ref _creationToplistBox, value.ThrowIfNull(nameof(value)));
        }

        public ObservableCollection<ToplistItem> Items { get; }
           = new ObservableCollection<ToplistItem>();


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

            if (string.IsNullOrWhiteSpace(toplistItem.Name) &&
                Items.Contains(toplistItem))
            {
                RemoveItem(toplistItem);
                return true;
            }

            return false;
        }
    }
}
