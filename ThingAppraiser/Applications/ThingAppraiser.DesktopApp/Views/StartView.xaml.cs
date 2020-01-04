using System.Collections.Generic;
using System.Windows.Controls;
using Acolyte.Assertions;
using Acolyte.Collections;
using MaterialDesignThemes.Wpf;
using Prism.Events;
using ThingAppraiser.DesktopApp.Domain;
using ThingAppraiser.DesktopApp.Domain.Messages;
using ThingAppraiser.DesktopApp.Models.Things;
using ThingAppraiser.DesktopApp.ViewModels;

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
            if (!(eventArgs.Session.Content is InputThingView inputThingView)) return;

            // Make sure that items list is clear when we start new dialog.
            inputThingView.ClearAllView();
        }

        private void InputThing_DialogClosing(object sender, DialogClosingEventArgs eventArgs)
        {
            if (Equals(eventArgs.Parameter, false)) return;

            if (!(eventArgs.Parameter is InputThingViewModel inputThingViewModel)) return;

            IReadOnlyList<string> thingList = inputThingViewModel.ThingList.ToReadOnlyList();
            if (thingList.IsNullOrEmpty()) return;

            var thingsData = ThingsDataToAppraise.Create(
                DataSource.InputThing, "User input", thingList
            );
            _eventAggregator
                .GetEvent<AppraiseInputThingsMessage>()
                .Publish(thingsData);
        }

        private void EnterData_DialogOpened(object sender, DialogOpenedEventArgs eventArgs)
        {
            if (!(eventArgs.Session.Content is EnterDataView enterDataView)) return;

            // Make sure that text box is clear when we start new dialog.
            enterDataView.Clear();
        }

        private void EnterData_DialogClosing(object sender, DialogClosingEventArgs eventArgs)
        {
            if (Equals(eventArgs.Parameter, false)) return;

            if (!(eventArgs.Parameter is EnterDataViewModel enterDataViewModel)) return;

            string googleDriveFilename = enterDataViewModel.Name;
            if (string.IsNullOrWhiteSpace(googleDriveFilename))
            {
                eventArgs.Cancel();
                return;
            }

            var thingsData = ThingsDataToAppraise.Create(
                DataSource.GoogleDrive, googleDriveFilename
            );
            _eventAggregator
                .GetEvent<AppraiseGoogleDriveThingsFileMessage>()
                .Publish(thingsData);
        }
    }
}
