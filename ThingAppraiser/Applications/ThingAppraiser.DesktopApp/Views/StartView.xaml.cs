using System.Windows.Controls;
using Prism.Events;
using ThingAppraiser.DesktopApp.ViewModels;
using ThingAppraiser.Extensions;

namespace ThingAppraiser.DesktopApp.Views
{
    /// <summary>
    /// Interaction logic for StartView.xaml
    /// </summary>
    public sealed partial class StartView : UserControl
    {
        public StartView(object dialogIdentifier, IEventAggregator eventAggregator)
        {
            dialogIdentifier.ThrowIfNull(nameof(dialogIdentifier));
            eventAggregator.ThrowIfNull(nameof(eventAggregator));

            InitializeComponent();

            DataContext = new StartViewModel(dialogIdentifier, eventAggregator);
        }
    }
}
