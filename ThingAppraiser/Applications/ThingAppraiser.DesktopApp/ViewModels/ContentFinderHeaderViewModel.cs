using System;
using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;
using ThingAppraiser.DesktopApp.Domain;
using ThingAppraiser.Logging;

namespace ThingAppraiser.DesktopApp.ViewModels
{
    internal sealed class ContentFinderHeaderViewModel : BindableBase
    {
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor<ContentFinderHeaderViewModel>();

        public ICommand ProcessContentDirectoryLocalDialogCommand { get; }
        
        public ICommand ProcessContentDirectoryFromDriveDialogCommand { get; }
        
        public ICommand OpenContentFinderResultsDialogCommand { get; }


        public ContentFinderHeaderViewModel()
        {
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
                // TODO: implement local content directory processing.
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
            MessageBoxHelper.ShowInfo("Work in progress.");
            // TODO: implement Google Drive content directory processing.
        }

        private void OpenContentFinderResults()
        {
            MessageBoxHelper.ShowInfo("Work in progress.");
            // TODO: implement loading content finder results from different sources.
        }
    }
}
