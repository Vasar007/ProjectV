using System.Collections.Generic;
using System.Linq;

namespace ThingAppraiser.DesktopApp.ViewModels
{
    internal class CreateToplistViewModel : ViewModelBase
    {
        private string _toplistName;

        private string _selectedToplistType;

        private string _selectedToplistFormat;

        public IReadOnlyList<string> ToplistType { get; } = new List<string>
        {
            "Score",
            "Simple"
        };

        public IReadOnlyList<string> ToplistFormat { get; } = new List<string>
        {
            "Forward",
            "Reverse"
        };

        public string ToplistName
        {
            get => _toplistName;
            set => SetProperty(ref _toplistName, value);
        }

        public string SelectedToplistType
        {
            get => _selectedToplistType;
            set => SetProperty(ref _selectedToplistType, value);
        }

        public string SelectedToplistFormat
        {
            get => _selectedToplistFormat;
            set => SetProperty(ref _selectedToplistFormat, value);
        }


        public CreateToplistViewModel()
        {
            SelectedToplistType = ToplistType.First();
            SelectedToplistFormat = ToplistFormat.First();
        }
    }
}
