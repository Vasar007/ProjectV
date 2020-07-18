using System.Windows;
using System.Windows.Controls;
using Acolyte.Assertions;
using Prism.Mvvm;

namespace ProjectV.DesktopApp.Models
{
    internal sealed class SceneItem : BindableBase
    {
        private string _name = default!; // Initializes throught property.
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value.ThrowIfNull(nameof(value)));
        }

        private UserControl _content = default!; // Initializes throught property.
        public UserControl Content
        {
            get => _content;
            set => SetProperty(ref _content, value.ThrowIfNull(nameof(value)));
        }

        private ScrollBarVisibility _horizontalScrollBarVisibilityRequirement;
        public ScrollBarVisibility HorizontalScrollBarVisibilityRequirement
        {
            get => _horizontalScrollBarVisibilityRequirement;
            set => SetProperty(ref _horizontalScrollBarVisibilityRequirement, value);
        }

        private ScrollBarVisibility _verticalScrollBarVisibilityRequirement;
        public ScrollBarVisibility VerticalScrollBarVisibilityRequirement
        {
            get => _verticalScrollBarVisibilityRequirement;
            set => SetProperty(ref _verticalScrollBarVisibilityRequirement, value);
        }

        private Thickness _marginRequirement = new Thickness(uniformLength: 16);
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
