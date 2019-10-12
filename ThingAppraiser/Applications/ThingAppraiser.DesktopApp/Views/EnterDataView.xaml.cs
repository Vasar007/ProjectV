using System.Windows.Controls;
using ThingAppraiser.DesktopApp.ViewModels;
using ThingAppraiser.Extensions;

namespace ThingAppraiser.DesktopApp.Views
{
    /// <summary>
    /// Interaction logic for EnterDataView.xaml
    /// </summary>
    public sealed partial class EnterDataView : UserControl
    {
        public EnterDataView(string hintText)
        {
            hintText.ThrowIfNullOrEmpty(nameof(hintText));

            InitializeComponent();

            DataContext = new EnterDataViewModel(hintText);
        }
    }
}
