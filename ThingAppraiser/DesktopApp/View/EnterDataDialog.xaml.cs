using System.Windows.Controls;
using DesktopApp.ViewModel;

namespace DesktopApp.View
{
    /// <summary>
    /// Interaction logic for InputDataDialog.xaml
    /// </summary>
    public partial class CEnterDataDialog : UserControl
    {
        public CEnterDataDialog()
        {
            InitializeComponent();

            DataContext = new CEnterDataDialogViewModel();
        }
    }
}
