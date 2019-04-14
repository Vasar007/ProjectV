using System.Windows.Controls;
using DesktopApp.ViewModel;

namespace DesktopApp.View
{
    /// <summary>
    /// Interaction logic for StartView.xaml
    /// </summary>
    public partial class CStartControl : UserControl
    {
        public CStartControl()
        {
            InitializeComponent();

            DataContext = new CStartControlViewModel(StartDialogHost.Identifier);
        }
    }
}
