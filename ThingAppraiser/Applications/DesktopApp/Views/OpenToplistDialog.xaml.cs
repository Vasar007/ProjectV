using System.Windows.Controls;
using ThingAppraiser.DesktopApp.ViewModels;

namespace ThingAppraiser.DesktopApp.Views
{
    /// <summary>
    /// Interaction logic for OpenToplistDialog.xaml
    /// </summary>
    public partial class OpenToplistDialog : UserControl
    {
        public OpenToplistDialog(object dialogIdentifier)
        {
            dialogIdentifier.ThrowIfNull(nameof(dialogIdentifier));

            InitializeComponent();

            DataContext = new OpenToplistViewModel(dialogIdentifier);
        }
    }
}
