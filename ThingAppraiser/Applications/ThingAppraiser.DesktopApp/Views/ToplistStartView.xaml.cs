using System.Windows.Controls;
using Prism.Events;
using ThingAppraiser.DesktopApp.ViewModels;
using ThingAppraiser.Extensions;

namespace ThingAppraiser.DesktopApp.Views
{
    /// <summary>
    /// Interaction logic for ToplistStartView.xaml
    /// </summary>
    public sealed partial class ToplistStartView : UserControl
    {
        public ToplistStartView(object dialogIdentifier, IEventAggregator eventAggregator)
        {
            dialogIdentifier.ThrowIfNull(nameof(dialogIdentifier));
            eventAggregator.ThrowIfNull(nameof(eventAggregator));

            InitializeComponent();

            DataContext = new ToplistStartViewModel(dialogIdentifier, eventAggregator);
        }
    }
}
