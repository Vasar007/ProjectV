using System.Linq;
using System.Windows.Controls;
using MaterialDesignThemes.Wpf;
using Prism.Events;
using ThingAppraiser.DesktopApp.Domain.Messages;
using ThingAppraiser.DesktopApp.ViewModels;
using ThingAppraiser.Extensions;

namespace ThingAppraiser.DesktopApp.Views
{
    /// <summary>
    /// Interaction logic for StartView.xaml
    /// </summary>
    public sealed partial class StartView : UserControl
    {
        private readonly IEventAggregator _eventAggregator;


        public StartView(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator.ThrowIfNull(nameof(eventAggregator));

            InitializeComponent();
        }

        private void InputThing_DialogOpened(object sender, DialogOpenedEventArgs eventArgs)
        {
            if (!(eventArgs.Session.Content is ContentControl contentControl)) return;
            if (!(contentControl.Content is InputThingView inputThingDialog)) return;
            if (!(inputThingDialog.DataContext is InputThingViewModel inputThingViewModel)) return;

            // Make sure that items list is clear when we start new dialog.
            inputThingViewModel.ThingList.Clear();
        }

        private void InputThing_DialogClosing(object sender, DialogClosingEventArgs eventArgs)
        {
            if (Equals(eventArgs.Parameter, false)) return;

            if (!(eventArgs.Parameter is InputThingViewModel inputThingViewModel)) return;

            if (inputThingViewModel.ThingList.IsNullOrEmpty()) return;

            _eventAggregator.GetEvent<AppraiseInputThingsMessage>()
                .Publish(inputThingViewModel.ThingList.ToList());
        }

        private void EnterData_DialogOpened(object sender, DialogOpenedEventArgs eventArgs)
        {
            if (!(eventArgs.Session.Content is ContentControl contentControl)) return;
            if (!(contentControl.Content is EnterDataView enterDataDialog)) return;
            if (!(enterDataDialog.DataContext is EnterDataViewModel enterDataViewModel)) return;

            // Make sure that text box is clear when we start new dialog.
            enterDataViewModel.Name = string.Empty;
            enterDataDialog.NameTextBox.Clear();
        }

        private void EnterData_DialogClosing(object sender, DialogClosingEventArgs eventArgs)
        {
            if (Equals(eventArgs.Parameter, false)) return;

            if (!(eventArgs.Parameter is EnterDataViewModel enterDataViewModel)) return;

            if (string.IsNullOrWhiteSpace(enterDataViewModel.Name))
            {
                eventArgs.Cancel();
                return;
            }

            _eventAggregator.GetEvent<AppraiseGoogleDriveThingsFileMessage>()
                .Publish(enterDataViewModel.Name);
        }
    }
}
