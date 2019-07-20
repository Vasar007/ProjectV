using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ThingAppraiser.DesktopApp.Models.Toplists
{
    // TODO: implement add, remove and other methods to wotrk with items.
    internal class Toplist : ModelBase
    {
        private string _name;

        private string _type;

        private string _format;

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value.ThrowIfNull(nameof(value)));
        }

        public string Type
        {
            get => _type;
            set => SetProperty(ref _type, value.ThrowIfNull(nameof(value)));
        }

        public string Format
        {
            get => _format;
            set => SetProperty(ref _format, value.ThrowIfNull(nameof(value)));
        }

        public ObservableCollection<ToplistItem> Items { get; private set; }


        public Toplist(string name, string type, string format)
        {
            Name = name;
            Type = type;
            Format = format;

            Items = new ObservableCollection<ToplistItem>();

            Items.Add(new ToplistItem("Name1", 1));
            Items.Add(new ToplistItem("Name2", 2));
            Items.Add(new ToplistItem("Name3", 3));
            Items.Add(new ToplistItem("Name4", 4));
            Items.Add(new ToplistItem("Name5", 5));
        }
    }
}
