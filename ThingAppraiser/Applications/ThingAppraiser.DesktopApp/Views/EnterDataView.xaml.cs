using System.Windows.Controls;
using ThingAppraiser.DesktopApp.ViewModels;

namespace ThingAppraiser.DesktopApp.Views
{
    /// <summary>
    /// Interaction logic for EnterDataView.xaml
    /// </summary>
    public sealed partial class EnterDataView : UserControl
    {
        public EnterDataView()
        {
            InitializeComponent();
        }

        public void Clear()
        {
            if (!(DataContext is EnterDataViewModel enterDataViewModel)) return;

            enterDataViewModel.Name = string.Empty;
            NameTextBox.Clear();
        }
    }
}
