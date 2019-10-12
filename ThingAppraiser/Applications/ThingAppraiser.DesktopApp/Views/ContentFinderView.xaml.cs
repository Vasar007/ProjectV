using System.Windows.Controls;
using ThingAppraiser.DesktopApp.ViewModels;

namespace ThingAppraiser.DesktopApp.Views
{
    /// <summary>
    /// Interaction logic for ContentFinderView.xaml
    /// </summary>
    public partial class ContentFinderView : UserControl
    {
        public ContentFinderView()
        {
            InitializeComponent();

            DataContext = new ContentFinderViewModel();
        }
    }
}
