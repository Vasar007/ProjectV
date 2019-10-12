using System;
using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;
using ThingAppraiser.DesktopApp.Domain;
using ThingAppraiser.Extensions;
using ThingAppraiser.Logging;

namespace ThingAppraiser.DesktopApp.ViewModels
{
    internal sealed class ContentFinderViewModel : BindableBase
    {
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor<ContentFinderViewModel>();

        public ICommand OpenFileDialogCommand { get; }


        public ContentFinderViewModel()
        {
            OpenFileDialogCommand = new DelegateCommand(OpenContentDirectoryCommand);
        }

        private void OpenContentDirectoryCommand()
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
                // TODO: implement content directory processing.
                MessageBoxHelper.ShowInfo("Work in progress.");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception occurred during content directory processing.");
                MessageBoxHelper.ShowError(ex.Message);
            }
        }
    }
}
