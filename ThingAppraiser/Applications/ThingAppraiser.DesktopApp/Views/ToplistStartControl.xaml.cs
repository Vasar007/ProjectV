using System.Windows.Controls;
using ThingAppraiser.DesktopApp.ViewModels;
using ThingAppraiser.Extensions;

namespace ThingAppraiser.DesktopApp.Views
{
    /// <summary>
    /// Interaction logic for ToplistStartControl.xaml
    /// </summary>
    public sealed partial class ToplistStartControl : UserControl
    {
        public ToplistStartControl(object dialogIdentifier)
        {
            dialogIdentifier.ThrowIfNull(nameof(dialogIdentifier));

            InitializeComponent();

            DataContext = new ToplistStartViewModel(dialogIdentifier);
        }
    }
}
