using System.Windows.Controls;
using Acolyte.Assertions;
using Prism.Events;
using Prism.Mvvm;
using ThingAppraiser.DesktopApp.Domain.Messages;
using ThingAppraiser.DesktopApp.Models.ContentDirectories;
using ThingAppraiser.DesktopApp.Views;

namespace ThingAppraiser.DesktopApp.ViewModels
{
    internal sealed class ContentDirectoriesResultsViewModel : BindableBase
    {
        private readonly IEventAggregator _eventAggregator;

        private ContentControl _contentDirectoryResult;
        public ContentControl ContentDirectoryResult
        {
            get => _contentDirectoryResult;
            set => SetProperty(ref _contentDirectoryResult, value.ThrowIfNull(nameof(value)));
        }


        public ContentDirectoriesResultsViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator.ThrowIfNull(nameof(eventAggregator));

            _eventAggregator
                .GetEvent<UpdateContentDirectoryInfoMessage>()
                .Subscribe(Update);

            _contentDirectoryResult = new ContentControl();
        }

        public void Update(ContentDirectoryInfo directoryInfo)
        {
            directoryInfo.ThrowIfNull(nameof(directoryInfo));

            ContentDirectoryResult.Content = new ContentDirectoryInfoView
            {
                DataContext = directoryInfo
            };
        }
    }
}
