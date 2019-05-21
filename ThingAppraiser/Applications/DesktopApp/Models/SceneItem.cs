using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using ThingAppraiser.DesktopApp.Domain;

namespace ThingAppraiser.DesktopApp.Models
{
    internal class SceneItem : INotifyPropertyChanged
    {
        private string _name;

        private UserControl _content;

        private ScrollBarVisibility _horizontalScrollBarVisibilityRequirement;

        private ScrollBarVisibility _verticalScrollBarVisibilityRequirement;

        private Thickness _marginRequirement = new Thickness(16);

        public string Name
        {
            get => _name;
            set => this.MutateVerbose(ref _name, value, RaisePropertyChanged());
        }

        public UserControl Content
        {
            get => _content;
            set => this.MutateVerbose(ref _content, value, RaisePropertyChanged());
        }

        public ScrollBarVisibility HorizontalScrollBarVisibilityRequirement
        {
            get => _horizontalScrollBarVisibilityRequirement;
            set => this.MutateVerbose(ref _horizontalScrollBarVisibilityRequirement, value,
                                      RaisePropertyChanged());
        }

        public ScrollBarVisibility VerticalScrollBarVisibilityRequirement
        {
            get => _verticalScrollBarVisibilityRequirement;
            set => this.MutateVerbose(ref _verticalScrollBarVisibilityRequirement, value,
                                      RaisePropertyChanged());
        }

        public Thickness MarginRequirement
        {
            get => _marginRequirement;
            set => this.MutateVerbose(ref _marginRequirement, value, RaisePropertyChanged());
        }


        public SceneItem(string name, UserControl content)
        {
            Name = name;
            Content = content;
        }

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        private Action<PropertyChangedEventArgs> RaisePropertyChanged()
        {
            return args => PropertyChanged?.Invoke(this, args);
        }

        #endregion
    }
}
