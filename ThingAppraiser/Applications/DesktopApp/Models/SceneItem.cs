using System.Windows;
using System.Windows.Controls;

namespace ThingAppraiser.DesktopApp.Models
{
    internal class SceneItem : ModelBase
    {
        private string _name;

        private UserControl _content;

        private ScrollBarVisibility _horizontalScrollBarVisibilityRequirement;

        private ScrollBarVisibility _verticalScrollBarVisibilityRequirement;

        private Thickness _marginRequirement = new Thickness(16);

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value.ThrowIfNull(nameof(value)));
        }

        public UserControl Content
        {
            get => _content;
            set => SetProperty(ref _content, value.ThrowIfNull(nameof(value)));
        }

        public ScrollBarVisibility HorizontalScrollBarVisibilityRequirement
        {
            get => _horizontalScrollBarVisibilityRequirement;
            set => SetProperty(ref _horizontalScrollBarVisibilityRequirement, value);
        }

        public ScrollBarVisibility VerticalScrollBarVisibilityRequirement
        {
            get => _verticalScrollBarVisibilityRequirement;
            set => SetProperty(ref _verticalScrollBarVisibilityRequirement, value);
        }

        public Thickness MarginRequirement
        {
            get => _marginRequirement;
            set => SetProperty(ref _marginRequirement, value);
        }


        public SceneItem(string name, UserControl content)
        {
            Name = name;
            Content = content;
        }
    }
}
