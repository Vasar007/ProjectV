using System.Windows.Controls;
using ThingAppraiser.DesktopApp.ViewModels;

namespace ThingAppraiser.DesktopApp.Views
{
    /// <summary>
    /// Interaction logic for ToplistEditorControl.xaml
    /// </summary>
    public partial class ToplistEditorControl : UserControl
    {
        public ToplistEditorControl()
        {
            InitializeComponent();

            DataContext = new ToplistEditorViewModel();
        }
    }
}
