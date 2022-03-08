﻿using System.Collections.Generic;
using System.Windows.Controls;
using Acolyte.Assertions;
using Acolyte.Linq;
using MaterialDesignThemes.Wpf;
using Prism.Events;
using ProjectV.DesktopApp.Domain;
using ProjectV.DesktopApp.Domain.Messages;
using ProjectV.DesktopApp.Models.Things;
using ProjectV.DesktopApp.ViewModels;

namespace ProjectV.DesktopApp.Views
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
            if (eventArgs.Session.Content is not InputThingView inputThingView) return;

            // Make sure that items list is clear when we start new dialog.
            inputThingView.ClearAllView();
        }

        private void InputThing_DialogClosing(object sender, DialogClosingEventArgs eventArgs)
        {
            if (Equals(eventArgs.Parameter, false)) return;

            if (eventArgs.Parameter is not InputThingViewModel inputThingViewModel) return;

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
            if (eventArgs.Session.Content is not EnterDataView enterDataView) return;

            // Make sure that text box is clear when we start new dialog.
            enterDataView.Clear();
        }

        private void EnterData_DialogClosing(object sender, DialogClosingEventArgs eventArgs)
        {
            if (Equals(eventArgs.Parameter, false)) return;

            if (eventArgs.Parameter is not EnterDataViewModel enterDataViewModel) return;

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
