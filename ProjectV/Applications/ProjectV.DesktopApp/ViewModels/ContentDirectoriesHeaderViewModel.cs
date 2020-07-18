using System.Windows.Input;
using Acolyte.Assertions;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using ProjectV.DesktopApp.Domain;

namespace ProjectV.DesktopApp.ViewModels
{
    internal sealed class ContentDirectoriesHeaderViewModel : BindableBase
    {
        private readonly IEventAggregator _eventAggregator;

        public ICommand ProcessContentDirectoryFromDriveDialogCommand { get; }

        public ICommand OpenContentFinderResultsDialogCommand { get; }


        public ContentDirectoriesHeaderViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator.ThrowIfNull(nameof(eventAggregator));

            ProcessContentDirectoryFromDriveDialogCommand = new DelegateCommand(
                ProcessContentDirectoryFromDrive
            );
            OpenContentFinderResultsDialogCommand = new DelegateCommand(
                OpenContentFinderResults
            );
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
