using System.Windows.Controls;
using ThingAppraiser.DesktopApp.ViewModels;
using ThingAppraiser.Extensions;

namespace ThingAppraiser.DesktopApp.Views
{
    /// <summary>
    /// Interaction logic for EnterDataDialog.xaml
    /// </summary>
    public sealed partial class EnterDataDialog : UserControl
    {
        public EnterDataDialog(string hintText)
        {
            hintText.ThrowIfNullOrEmpty(nameof(hintText));

            InitializeComponent();

            DataContext = new EnterDataViewModel(hintText);
        }
    }
}
