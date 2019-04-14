using System.Windows.Controls;
using DesktopApp.ViewModel;

namespace DesktopApp.View
{
    /// <summary>
    /// Interaction logic for InputThing.xaml
    /// </summary>
    public partial class CInputThingDialog : UserControl
    {
        public CInputThingDialog()
        {
            InitializeComponent();

            DataContext = new CInputThingViewModel(InputThingDialogHost.Identifier,
                                                   InputThingDialogHost.DialogContent);
        }
    }
}
