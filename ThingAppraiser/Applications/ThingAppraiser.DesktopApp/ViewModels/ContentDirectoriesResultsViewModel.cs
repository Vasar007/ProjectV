using System;
using System.Windows.Controls;
using Prism.Events;
using Prism.Mvvm;
using ThingAppraiser.DesktopApp.Domain;
using ThingAppraiser.DesktopApp.Domain.Messages;
using ThingAppraiser.DesktopApp.Models.ContentDirectories;
using ThingAppraiser.DesktopApp.Views;
using ThingAppraiser.Extensions;
using ThingAppraiser.Logging;

namespace ThingAppraiser.DesktopApp.ViewModels
{
    internal sealed class ContentDirectoriesResultsViewModel : BindableBase
    {
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor<ContentDirectoriesResultsViewModel>();

        private readonly IEventAggregator _eventAggregator;

        private readonly ContentFinderWrapper _contentFinder;

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
                .GetEvent<ProcessContentDirectoryMessage>()
                .Subscribe(ProcessContentDirectory);

            _contentFinder = new ContentFinderWrapper();
            _contentDirectoryResult = new ContentControl();
        }

        private void ProcessContentDirectory(string contentDirectoryPath)
        {
            try
            {
                ContentDirectoryInfo result = _contentFinder.GetAllDirectoryContent(
                    contentDirectoryPath, ContentTypeToFind.Text
                );

                ContentDirectoryResult.Content = new ContentDirectoryView { DataContext = result };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception occurred during content directory processing.");
                MessageBoxProvider.ShowError(ex.Message);
            }
        }
    }
}
