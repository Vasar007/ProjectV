using System.Windows.Controls;
using ThingAppraiser.DesktopApp.ViewModels;

namespace ThingAppraiser.DesktopApp.Views
{
    /// <summary>
    /// Interaction logic for InputThingView.xaml
    /// </summary>
    public sealed partial class InputThingView : UserControl
    {
        public InputThingView()
        {
            InitializeComponent();

            DataContext = new InputThingViewModel(InputThingDialogHost.Identifier,
                                                  InputThingDialogHost.DialogContent);
        }
    }
}
