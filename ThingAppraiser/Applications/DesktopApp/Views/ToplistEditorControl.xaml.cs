using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Navigation;
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
