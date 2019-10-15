using System.Windows.Controls;
using MaterialDesignThemes.Wpf;
using ThingAppraiser.DesktopApp.ViewModels;

namespace ThingAppraiser.DesktopApp.Views
{
    /// <summary>
    /// Interaction logic for InputThingView.xaml
    /// </summary>
    public sealed partial class InputThingView : UserControl
    {
        public InputThingView()
        {
            InitializeComponent();
        }

        private void EnterThingName_DialogOpened(object sender,
            DialogOpenedEventArgs eventArgs)
        {
            // Make sure that text box is clear when we start new dialog.
            ThingNameTextBox.Clear();
        }

        private void EnterThingName_DialogClosing(object sender,
            DialogClosingEventArgs eventArgs)
        {
            if (Equals(eventArgs.Parameter, false)) return;

            if (!(DataContext is InputThingViewModel inputThingViewModel)) return;

            string thingName = ThingNameTextBox.Text;
            if (string.IsNullOrWhiteSpace(thingName)) return;

            inputThingViewModel.ThingList.Add(thingName.Trim());
            ThingNameTextBox.Clear();
        }
    }
}
