using System.Collections.Generic;
using System.Linq;

namespace ThingAppraiser.DesktopApp.ViewModels
{
    internal class CreateToplistViewModel : ViewModelBase
    {
        private string _toplistName;

        private string _selectedToplistType;

        private string _selectedToplistFormat;

        // TODO: transform this strings to enum type.
        public IReadOnlyList<string> ToplistType { get; } = new List<string>
        {
            "Score",
            "Simple"
        };

        // TODO: transform this strings to enum type.
        public IReadOnlyList<string> ToplistFormat { get; } = new List<string>
        {
            "Forward",
            "Reverse"
        };

        public string ToplistName
        {
            get => _toplistName;
            set => SetProperty(ref _toplistName, value.ThrowIfNull(nameof(value)));
        }

        public string SelectedToplistType
        {
            get => _selectedToplistType;
            set => SetProperty(ref _selectedToplistType, value.ThrowIfNull(nameof(value)));
        }

        public string SelectedToplistFormat
        {
            get => _selectedToplistFormat;
            set => SetProperty(ref _selectedToplistFormat, value.ThrowIfNull(nameof(value)));
        }


        public CreateToplistViewModel()
        {
            SelectedToplistType = ToplistType.First();
            SelectedToplistFormat = ToplistFormat.First();
        }
    }
}
