using System.Windows.Controls;
using DesktopApp.ViewModel;

namespace DesktopApp.View
{
    /// <summary>
    /// Interaction logic for ProgressDialog.xaml
    /// </summary>
    public partial class CProgressDialog : UserControl
    {
        public CProgressDialog()
        {
            InitializeComponent();

            DataContext = new CProgressDialogViewModel();
        }
    }
}
