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

        public void Clear(bool clearItemList)
        {
            // Make sure that text box is clear when we start new dialog.
            if (!(DataContext is InputThingViewModel inputThingViewModel)) return;

            inputThingViewModel.ThingName = string.Empty;
            ThingNameTextBox.Clear();

            if (clearItemList)
            {
                inputThingViewModel.ThingList.Clear();
            }
        }

        private void EnterThingName_DialogOpened(object sender, DialogOpenedEventArgs eventArgs)
        {
            Clear(clearItemList: false);
        }

        private void EnterThingName_DialogClosing(object sender, DialogClosingEventArgs eventArgs)
        {
            if (Equals(eventArgs.Parameter, false)) return;

            if (!(DataContext is InputThingViewModel inputThingViewModel)) return;

            string thingName = inputThingViewModel.ThingName;
            if (string.IsNullOrWhiteSpace(thingName))
            {
                eventArgs.Cancel();
                return;
            }

            inputThingViewModel.ThingList.Add(thingName.Trim());
        }
    }
}
