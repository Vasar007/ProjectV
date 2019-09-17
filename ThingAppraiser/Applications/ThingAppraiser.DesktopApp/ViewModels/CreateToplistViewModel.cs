using System.Collections.Generic;
using System.Linq;
using ThingAppraiser.DesktopApp.Models.Toplists;

namespace ThingAppraiser.DesktopApp.ViewModels
{
    internal sealed class CreateToplistViewModel : ViewModelBase
    {
        private string _toplistName = string.Empty;

        private ToplistType _selectedToplistType;

        private ToplistFormat _selectedToplistFormat;

        public IReadOnlyList<ToplistType> ToplistTypes { get; } = new List<ToplistType>
        {
            ToplistType.Score,
            ToplistType.Simple
        };

        public IReadOnlyList<ToplistFormat> ToplistFormats { get; } = new List<ToplistFormat>
        {
            ToplistFormat.Forward,
            ToplistFormat.Reverse
        };

        public string ToplistName
        {
            get => _toplistName;
            set => SetProperty(ref _toplistName, value.ThrowIfNull(nameof(value)));
        }

        public ToplistType SelectedToplistType
        {
            get => _selectedToplistType;
            set => SetProperty(ref _selectedToplistType, value);
        }

        public ToplistFormat SelectedToplistFormat
        {
            get => _selectedToplistFormat;
            set => SetProperty(ref _selectedToplistFormat, value);
        }


        public CreateToplistViewModel()
        {
            SelectedToplistType = ToplistTypes.First();
            SelectedToplistFormat = ToplistFormats.First();
        }
    }
}
