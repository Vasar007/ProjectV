using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Acolyte.Assertions;
using Prism.Commands;
using Prism.Mvvm;
using ProjectV.DesktopApp.Models.ContentDirectories;
using ProjectV.Logging;

namespace ProjectV.DesktopApp.ViewModels
{
    internal sealed class ChooseContentDirectoryViewModel : BindableBase
    {
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor<ChooseContentDirectoryViewModel>();

        public IReadOnlyList<ContentTypeToFind> ContentTypes =>
            ContentDirectoryOptions.ContentTypes;

        private string _directoryPath;
        public string DirectoryPath
        {
            get => _directoryPath;
            set => SetProperty(ref _directoryPath, value.ThrowIfNull(nameof(value)));
        }

        private ContentTypeToFind _selectedContentType;
        public ContentTypeToFind SelectedContentType
        {
            get => _selectedContentType;
            set => SetProperty(ref _selectedContentType, value);
        }

        public ICommand OpenContentDirectoryDialogCommand { get; }


        public ChooseContentDirectoryViewModel()
        {
            _directoryPath = string.Empty;
            SelectedContentType = ContentTypes.First();

            OpenContentDirectoryDialogCommand = new DelegateCommand(OpenLocalContentDirectory);
        }

        private void OpenLocalContentDirectory()
        {
            string? contentDirectoryPath = ExecutableDialogs.ExecuteOpenContentDirectoryDialog();
            if (string.IsNullOrWhiteSpace(contentDirectoryPath))
            {
                _logger.Warn(
                    "Skipping openning content directory because got an empty path value."
                );
                return;
            }

            DirectoryPath = contentDirectoryPath;
        }
    }
}
