using System.Windows.Controls;
using ThingAppraiser.DesktopApp.ViewModels;

namespace ThingAppraiser.DesktopApp.Views
{
    /// <summary>
    /// Interaction logic for InputThing.xaml
    /// </summary>
    public sealed partial class InputThingDialog : UserControl
    {
        public InputThingDialog()
        {
            InitializeComponent();

            DataContext = new InputThingViewModel(InputThingDialogHost.Identifier,
                                                  InputThingDialogHost.DialogContent);
        }
    }
}
