using System.Windows.Controls;
using ThingAppraiser.DesktopApp.ViewModels;

namespace ThingAppraiser.DesktopApp.Views
{
    /// <summary>
    /// Interaction logic for EnterDataDialog.xaml
    /// </summary>
    public partial class EnterDataDialog : UserControl
    {
        public EnterDataDialog(string hintText)
        {
            hintText.ThrowIfNullOrEmpty(nameof(hintText));

            InitializeComponent();

            DataContext = new EnterDataDialogViewModel(hintText);
        }
    }
}
