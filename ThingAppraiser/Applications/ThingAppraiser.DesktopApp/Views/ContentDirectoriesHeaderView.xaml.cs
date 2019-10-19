using System.Windows.Controls;
using MaterialDesignThemes.Wpf;
using Prism.Events;
using ThingAppraiser.DesktopApp.Domain.Messages;
using ThingAppraiser.DesktopApp.Models.ContentDirectories;
using ThingAppraiser.DesktopApp.ViewModels;
using ThingAppraiser.Extensions;

namespace ThingAppraiser.DesktopApp.Views
{
    /// <summary>
    /// Interaction logic for ContentDirectoriesHeaderView.xaml
    /// </summary>
    public partial class ContentDirectoriesHeaderView : UserControl
    {
        private readonly IEventAggregator _eventAggregator;


        public ContentDirectoriesHeaderView(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator.ThrowIfNull(nameof(eventAggregator));

            InitializeComponent();
        }

        private void CreateToplist_DialogOpened(object sender, DialogOpenedEventArgs eventArgs)
        {
            if (!(eventArgs.Session.Content is ChooseContentDirectoryView chooseContentDirectoryView)) return;

            // Make sure that text box is clear when we start new dialog.
            chooseContentDirectoryView.Clear();
        }

        private void CreateToplist_DialogClosing(object sender, DialogClosingEventArgs eventArgs)
        {
            if (Equals(eventArgs.Parameter, false)) return;

            if (!(eventArgs.Parameter is ChooseContentDirectoryViewModel chooseContentDirectoryViewModel)) return;

            string directoryPath = chooseContentDirectoryViewModel.DirectoryPath;
            if (string.IsNullOrWhiteSpace(directoryPath))
            {
                eventArgs.Cancel();
                return;
            }

            ContentTypeToFind contentType = chooseContentDirectoryViewModel.SelectedContentType;

            var parameters = new ContentDirectoryParametersInfo(directoryPath, contentType);
            _eventAggregator
                .GetEvent<ProcessContentDirectoryMessage>()
                .Publish(parameters);
        }
    }
}
