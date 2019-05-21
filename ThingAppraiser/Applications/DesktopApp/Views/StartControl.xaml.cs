using System.Windows.Controls;
using ThingAppraiser.DesktopApp.ViewModels;

namespace ThingAppraiser.DesktopApp.Views
{
    /// <summary>
    /// Interaction logic for StartView.xaml
    /// </summary>
    public partial class StartControl : UserControl
    {
        public StartControl(object dialogIdentifier)
        {
            InitializeComponent();

            DataContext = new StartControlViewModel(dialogIdentifier);
        }
    }
}
