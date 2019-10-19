using System.Collections.Generic;
using System.Linq;
using ThingAppraiser.DesktopApp.Models.Toplists;
using ThingAppraiser.Extensions;
using Prism.Mvvm;

namespace ThingAppraiser.DesktopApp.ViewModels
{
    internal sealed class CreateToplistViewModel : BindableBase
    {
        public IReadOnlyList<ToplistType> ToplistTypes => ToplistOptions.ToplistTypes;

        public IReadOnlyList<ToplistFormat> ToplistFormats => ToplistOptions.ToplistFormats;

        private string _toplistName;
        public string ToplistName
        {
            get => _toplistName;
            set => SetProperty(ref _toplistName, value.ThrowIfNull(nameof(value)));
        }

        private ToplistType _selectedToplistType;
        public ToplistType SelectedToplistType
        {
            get => _selectedToplistType;
            set => SetProperty(ref _selectedToplistType, value);
        }

        private ToplistFormat _selectedToplistFormat;
        public ToplistFormat SelectedToplistFormat
        {
            get => _selectedToplistFormat;
            set => SetProperty(ref _selectedToplistFormat, value);
        }


        public CreateToplistViewModel()
        {
            _toplistName = string.Empty;

            SelectedToplistType = ToplistTypes.First();
            SelectedToplistFormat = ToplistFormats.First();
        }
    }
}
