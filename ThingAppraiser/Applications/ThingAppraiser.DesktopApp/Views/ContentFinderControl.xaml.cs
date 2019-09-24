using System.Windows.Controls;
using ThingAppraiser.DesktopApp.ViewModels;

namespace ThingAppraiser.DesktopApp.Views
{
    /// <summary>
    /// Interaction logic for ContentFinderControl.xaml
    /// </summary>
    public partial class ContentFinderControl : UserControl
    {
        public ContentFinderControl(object dialogIdentifier)
        {
            dialogIdentifier.ThrowIfNull(nameof(dialogIdentifier));

            InitializeComponent();

            DataContext = new ContentFinderViewModel(dialogIdentifier);
        }
    }
}
