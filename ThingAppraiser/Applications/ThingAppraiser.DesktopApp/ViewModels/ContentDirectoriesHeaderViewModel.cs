using System.Windows.Input;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using ThingAppraiser.DesktopApp.Domain;
using ThingAppraiser.DesktopApp.Domain.Messages;
using ThingAppraiser.Extensions;
using ThingAppraiser.Logging;

namespace ThingAppraiser.DesktopApp.ViewModels
{
    internal sealed class ContentDirectoriesHeaderViewModel : BindableBase
    {
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor<ContentDirectoriesHeaderViewModel>();

        private readonly IEventAggregator _eventAggregator;

        public ICommand ProcessContentDirectoryLocalDialogCommand { get; }
        
        public ICommand ProcessContentDirectoryFromDriveDialogCommand { get; }
        
        public ICommand OpenContentFinderResultsDialogCommand { get; }


        public ContentDirectoriesHeaderViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator.ThrowIfNull(nameof(eventAggregator));

            ProcessContentDirectoryLocalDialogCommand = new DelegateCommand(
                OpenLocalContentDirectory
            );
            ProcessContentDirectoryFromDriveDialogCommand = new DelegateCommand(
                ProcessContentDirectoryFromDrive
            );
            OpenContentFinderResultsDialogCommand = new DelegateCommand(
                OpenContentFinderResults
            );

        }

        private void OpenLocalContentDirectory()
        {
            string? contentDirectoryPath = ExecutableDialogs.ExecuteOpenContentDirectoryDialog();
            if (string.IsNullOrWhiteSpace(contentDirectoryPath))
            {
                _logger.Info(
                    "Skipping openning content directory because got an empty path value."
                );
                return;
            }

            _eventAggregator
                .GetEvent<ProcessContentDirectoryMessage>()
                .Publish(contentDirectoryPath);
        }

        private void ProcessContentDirectoryFromDrive()
        {
            // TODO: implement Google Drive content directory processing.
            MessageBoxProvider.ShowInfo("Work in progress.");
        }

        private void OpenContentFinderResults()
        {
            // TODO: implement loading content finder results from different sources.
            MessageBoxProvider.ShowInfo("Work in progress.");
        }
    }
}
