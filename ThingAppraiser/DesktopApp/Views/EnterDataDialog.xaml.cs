using System.Windows.Controls;
using ThingAppraiser.DesktopApp.ViewModels;

namespace ThingAppraiser.DesktopApp.Views
{
    /// <summary>
    /// Interaction logic for InputDataDialog.xaml
    /// </summary>
    public partial class EnterDataDialog : UserControl
    {
        public EnterDataDialog()
        {
            InitializeComponent();

            DataContext = new EnterDataDialogViewModel();
        }
    }
}
