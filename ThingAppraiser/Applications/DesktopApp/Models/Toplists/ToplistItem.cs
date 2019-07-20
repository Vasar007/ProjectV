﻿namespace ThingAppraiser.DesktopApp.Models.Toplists
{
    internal class ToplistItem : ModelBase
    {
        private string _name;

        private int _position;

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value.ThrowIfNull(nameof(value)));
        }

        public int Position
        {
            get => _position;
            set => SetProperty(ref _position, value);
        }


        public ToplistItem(string name, int position)
        {
            Name = name;
            Position = position;
        }
    }
}