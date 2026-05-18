using System.Collections.Generic;
using System.Collections.ObjectModel;
using Acolyte.Assertions;
using Prism.Mvvm;
using ProjectV.DesktopApp.Views;

namespace ProjectV.DesktopApp.Models.Toplists
{
    internal sealed class ToplistBlock : BindableBase
    {
        private string _title = default!; // Initializes throught property.
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value.ThrowIfNull(nameof(value)));
        }

        private int _number;
        public int Number
        {
            get => _number;
            set => SetProperty(ref _number, value);
        }

        private ToplistBoxView _creationToplistBox;
        public ToplistBoxView CreationToplistBox
        {
            get => _creationToplistBox;
            set => SetProperty(ref _creationToplistBox, value.ThrowIfNull(nameof(value)));
        }

        public ObservableCollection<ToplistItem> Items { get; private set; }
           = new ObservableCollection<ToplistItem>();


        public ToplistBlock(string title, int number)
        {
            Title = title;
            Number = number;

            // Initial item (always remains at the end of block).
            _creationToplistBox = new ToplistBoxView(new ToplistItem(string.Empty, null, this));
        }

        public void UpdateItems(IEnumerable<ToplistItem> items)
        {
            items.ThrowIfNull(nameof(items));

            Items = new ObservableCollection<ToplistItem>(items);
        }

        public bool AddOrUpdateItem(ToplistItem toplistItem)
        {
            toplistItem.ThrowIfNull(nameof(toplistItem));

            bool isNameNullOrWhitespace = string.IsNullOrWhiteSpace(toplistItem.Name);

            if (Items.Contains(toplistItem))
            {
                // Remove items without names. This action is required when user press "Enter" key.
                if (isNameNullOrWhitespace)
                {
                    RemoveItem(toplistItem);
                }

                return false;
            }

            if (isNameNullOrWhitespace) return false;

            Items.Add(toplistItem.Clone());
            return true;
        }

        public bool RemoveItem(ToplistItem toplistItem)
        {
            toplistItem.ThrowIfNull(nameof(toplistItem));

            return Items.Remove(toplistItem);
        }

        public bool DeleteItemIfNeeded(ToplistItem toplistItem)
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
