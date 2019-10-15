using System.Windows.Controls;
using ThingAppraiser.DesktopApp.ViewModels;

namespace ThingAppraiser.DesktopApp.Views
{
    /// <summary>
    /// Interaction logic for CreateToplistView.xaml
    /// </summary>
    public sealed partial class CreateToplistView : UserControl
    {
        public CreateToplistView()
        {
            InitializeComponent();
        }

        public void Clear()
        {
            if (!(DataContext is CreateToplistViewModel createToplistViewModel)) return;

            createToplistViewModel.ToplistName = string.Empty;
            ToplistNameTextBox.Clear();
        }
    }
}
