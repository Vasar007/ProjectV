using System.Windows.Controls;
using ProjectV.DesktopApp.ViewModels;

namespace ProjectV.DesktopApp.Views
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
            if (DataContext is not EnterDataViewModel enterDataViewModel) return;

            enterDataViewModel.Name = string.Empty;
            NameTextBox.Clear();
        }
    }
}
