using System;
using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;
using ThingAppraiser.DesktopApp.Domain;
using ThingAppraiser.DesktopApp.Models.ContentDirectories;
using ThingAppraiser.Logging;

namespace ThingAppraiser.DesktopApp.ViewModels
{
    internal sealed class ContentDirectoriesHeaderViewModel : BindableBase
    {
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor<ContentDirectoriesHeaderViewModel>();

        private readonly ContentFinderWrapper _contentFinder;

        public ICommand ProcessContentDirectoryLocalDialogCommand { get; }
        
        public ICommand ProcessContentDirectoryFromDriveDialogCommand { get; }
        
        public ICommand OpenContentFinderResultsDialogCommand { get; }


        public ContentDirectoriesHeaderViewModel()
        {
            _contentFinder = new ContentFinderWrapper();

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

            ProcessContentDirectory(contentDirectoryPath);
        }

        private void ProcessContentDirectory(string contentDirectoryPath)
        {
            try
            {
                ContentDirectoryInfo result = _contentFinder.GetAllDirectoryContent(
                    contentDirectoryPath, ContentTypeToFind.Text
                );

                result.PrintResultToOutput();

                // TODO: finish local content directory processing.
                MessageBoxProvider.ShowInfo("Work in progress.");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception occurred during content directory processing.");
                MessageBoxProvider.ShowError(ex.Message);
            }
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
