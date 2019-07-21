using System.Collections.ObjectModel;

namespace ThingAppraiser.DesktopApp.Models.Toplists
{
    internal abstract class ToplistBase : ModelBase
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

        public ObservableCollection<ToplistBlock> Blocks { get; }
            = new ObservableCollection<ToplistBlock>();


        public ToplistBase(string name, string type, string format)
        {
            Name = name;
            Type = type;
            Format = format;
        }
    }
}
