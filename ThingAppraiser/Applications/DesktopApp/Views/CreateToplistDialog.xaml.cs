using System.Windows.Controls;
using ThingAppraiser.DesktopApp.ViewModels;

namespace ThingAppraiser.DesktopApp.Views
{
    /// <summary>
    /// Interaction logic for CreateToplistDialog.xaml
    /// </summary>
    public partial class CreateToplistDialog : UserControl
    {
        public CreateToplistDialog()
        {
            InitializeComponent();

            DataContext = new CreateToplistViewModel();
        }
    }
}
