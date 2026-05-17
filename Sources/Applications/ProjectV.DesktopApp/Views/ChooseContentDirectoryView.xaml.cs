using System.Windows.Controls;
using ProjectV.DesktopApp.ViewModels;

namespace ProjectV.DesktopApp.Views
{
    /// <summary>
    /// Interaction logic for ChooseContentDirectoryView.xaml
    /// </summary>
    public partial class ChooseContentDirectoryView : UserControl
    {
        public ChooseContentDirectoryView()
        {
            InitializeComponent();
        }

        public void Clear()
        {
            if (DataContext is not ChooseContentDirectoryViewModel createToplistViewModel) return;

            createToplistViewModel.DirectoryPath = string.Empty;
            DirectoryPathTextBox.Clear();
        }
    }
}
