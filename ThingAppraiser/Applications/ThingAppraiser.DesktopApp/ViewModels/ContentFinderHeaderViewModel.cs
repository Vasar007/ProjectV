using System;
using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;
using ThingAppraiser.DesktopApp.Domain;
using ThingAppraiser.DesktopApp.Models.ContentDirectories;
using ThingAppraiser.Logging;

namespace ThingAppraiser.DesktopApp.ViewModels
{
    internal sealed class ContentFinderHeaderViewModel : BindableBase
    {
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor<ContentFinderHeaderViewModel>();

        private readonly ContentFinderWrapper _contentFinder;

        public ICommand ProcessContentDirectoryLocalDialogCommand { get; }
        
        public ICommand ProcessContentDirectoryFromDriveDialogCommand { get; }
        
        public ICommand OpenContentFinderResultsDialogCommand { get; }


        public ContentFinderHeaderViewModel()
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
                var result = _contentFinder.GetAllDirectoryContent(
                    contentDirectoryPath, ContentTypeToFind.Text
                );

                _contentFinder.PrintResultToOutput(result);

                // TODO: finish local content directory processing.
                MessageBoxHelper.ShowInfo("Work in progress.");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception occurred during content directory processing.");
                MessageBoxHelper.ShowError(ex.Message);
            }
        }

        private void ProcessContentDirectoryFromDrive()
        {
            // TODO: implement Google Drive content directory processing.
            MessageBoxHelper.ShowInfo("Work in progress.");
        }

        private void OpenContentFinderResults()
        {
            // TODO: implement loading content finder results from different sources.
            MessageBoxHelper.ShowInfo("Work in progress.");
        }
    }
}
