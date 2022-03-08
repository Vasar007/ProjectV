﻿using System.Windows.Controls;
using Acolyte.Assertions;
using MaterialDesignThemes.Wpf;
using Prism.Events;
using ProjectV.DesktopApp.Domain.Messages;
using ProjectV.DesktopApp.Models.Toplists;
using ProjectV.DesktopApp.ViewModels;

namespace ProjectV.DesktopApp.Views
{
    /// <summary>
    /// Interaction logic for ToplistHeaderView.xaml
    /// </summary>
    public sealed partial class ToplistHeaderView : UserControl
    {
        private readonly IEventAggregator _eventAggregator;


        public ToplistHeaderView(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator.ThrowIfNull(nameof(eventAggregator));

            InitializeComponent();
        }

        private void CreateToplist_DialogOpened(object sender, DialogOpenedEventArgs eventArgs)
        {
            if (eventArgs.Session.Content is not CreateToplistView createToplistView) return;

            // Make sure that text box is clear when we start new dialog.
            createToplistView.Clear();
        }

        private void CreateToplist_DialogClosing(object sender, DialogClosingEventArgs eventArgs)
        {
            if (Equals(eventArgs.Parameter, false)) return;

            if (eventArgs.Parameter is not CreateToplistViewModel createToplistViewModel) return;

            string toplistName = createToplistViewModel.ToplistName;
            if (string.IsNullOrWhiteSpace(toplistName))
            {
                eventArgs.Cancel();
                return;
            }

            ToplistType toplistType = createToplistViewModel.SelectedToplistType;
            ToplistFormat toplistFormat = createToplistViewModel.SelectedToplistFormat;

            var parameters = new ToplistParametersInfo(toplistName, toplistType, toplistFormat);
            _eventAggregator
                .GetEvent<ConstructToplistMessage>()
                .Publish(parameters);
        }
    }
}
