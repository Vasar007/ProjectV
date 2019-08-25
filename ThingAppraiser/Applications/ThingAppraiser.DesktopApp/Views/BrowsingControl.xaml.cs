using System.Windows.Controls;
using ThingAppraiser.DesktopApp.ViewModels;

namespace ThingAppraiser.DesktopApp.Views
{
    /// <summary>
    /// Interaction logic for BrowsingControl.xaml
    /// </summary>
    public sealed partial class BrowsingControl : UserControl
    {
        public BrowsingControl(ViewModelBase dataContext)
        {
            InitializeComponent();

            DataContext = dataContext.ThrowIfNull(nameof(dataContext));
        }
    }
}
