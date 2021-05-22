using System.Windows.Controls;
using Acolyte.Assertions;
using MaterialDesignThemes.Wpf;
using ProjectV.DesktopApp.ViewModels;

namespace ProjectV.DesktopApp.Views
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

        public void ClearTextBox()
        {
            if (DataContext is not InputThingViewModel inputThingViewModel) return;

            ClearInternal(inputThingViewModel);
        }

        public void ClearAllView()
        {
            if (DataContext is not InputThingViewModel inputThingViewModel) return;

            ClearInternal(inputThingViewModel);
            inputThingViewModel.ThingList.Clear();
        }

        private void ClearInternal(InputThingViewModel inputThingViewModel)
        {
            inputThingViewModel.ThrowIfNull(nameof(inputThingViewModel));

            inputThingViewModel.ThingName = string.Empty;
            ThingNameTextBox.Clear();
        }

        private void EnterThingName_DialogOpened(object sender, DialogOpenedEventArgs eventArgs)
        {
            // Make sure that text box is clear when we start new dialog.
            ClearTextBox();
        }

        private void EnterThingName_DialogClosing(object sender, DialogClosingEventArgs eventArgs)
        {
            if (Equals(eventArgs.Parameter, false)) return;

            if (DataContext is not InputThingViewModel inputThingViewModel) return;

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
