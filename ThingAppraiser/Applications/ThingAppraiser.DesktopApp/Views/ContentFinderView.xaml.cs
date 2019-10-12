using System.Windows.Controls;
using ThingAppraiser.DesktopApp.ViewModels;
using ThingAppraiser.Extensions;

namespace ThingAppraiser.DesktopApp.Views
{
    /// <summary>
    /// Interaction logic for ContentFinderView.xaml
    /// </summary>
    public partial class ContentFinderView : UserControl
    {
        public ContentFinderView(object dialogIdentifier)
        {
            dialogIdentifier.ThrowIfNull(nameof(dialogIdentifier));

            InitializeComponent();

            DataContext = new ContentFinderViewModel(dialogIdentifier);
        }
    }
}
