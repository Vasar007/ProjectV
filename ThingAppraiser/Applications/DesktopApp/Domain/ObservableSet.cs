using System;
using System.Collections.ObjectModel;

namespace ThingAppraiser.DesktopApp.Domain
{
    internal sealed class ObservableSet<T> : ObservableCollection<T>
    {
        public ObservableSet()
        {
        }

        protected override void InsertItem(int index, T item)
        {
            if (Contains(item))
            {
                throw new ArgumentException("Item already exists.", nameof(item));
            }

            base.InsertItem(index, item);
        }

        protected override void SetItem(int index, T item)
        {
            int i = IndexOf(item);
            if (i >= 0 && i != index)
            {
                throw new ArgumentException("Item already exists.", nameof(item));
            }

            base.SetItem(index, item);
        }
    }
}
