using System.Windows.Controls;
using ThingAppraiser.DesktopApp.ViewModels;

namespace ThingAppraiser.DesktopApp.Views
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
            if (!(DataContext is ChooseContentDirectoryViewModel createToplistViewModel)) return;

            createToplistViewModel.DirectoryPath = string.Empty;
            DirectoryPathTextBox.Clear();
        }
    }
}
