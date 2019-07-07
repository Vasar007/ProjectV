using System.Windows.Controls;
using ThingAppraiser.DesktopApp.ViewModels;

namespace ThingAppraiser.DesktopApp.Views
{
    /// <summary>
    /// Interaction logic for ToplistEditorStartControl.xaml
    /// </summary>
    public partial class ToplistEditorStartControl : UserControl
    {
        public ToplistEditorStartControl(object dialogIdentifier)
        {
            dialogIdentifier.ThrowIfNull(nameof(dialogIdentifier));

            InitializeComponent();

            DataContext = new ToplistEditorViewModel(dialogIdentifier);
        }
    }
}
