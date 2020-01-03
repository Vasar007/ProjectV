using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using ThingAppraiser.DesktopApp.Domain;
using ThingAppraiser.DesktopApp.Domain.Commands;
using ThingAppraiser.DesktopApp.Domain.Messages;
using ThingAppraiser.DesktopApp.Models;
using ThingAppraiser.DesktopApp.Models.ContentDirectories;
using ThingAppraiser.DesktopApp.Models.DataSuppliers;
using ThingAppraiser.DesktopApp.Models.Things;
using ThingAppraiser.DesktopApp.Views;
using ThingAppraiser.Extensions;
using ThingAppraiser.Logging;
using ThingAppraiser.Models.Internal;
using ThingAppraiser.Models.WebService;

namespace ThingAppraiser.DesktopApp.ViewModels
{
    internal sealed class MainWindowViewModel : BindableBase
    {
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor<MainWindowViewModel>();

        private readonly IEventAggregator _eventAggregator;

        private readonly SceneItemsCollection _scenes;

        private string _title;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value.ThrowIfNull(nameof(value)));
        }

        private bool _isNotBusy;
        public bool IsNotBusy
        {
            get => _isNotBusy;
            private set => SetProperty(ref _isNotBusy, value);
        }

        // Initializes throught property (in SelectedSceneItem when ChangeScene called in ctor).
        private UserControl _currentContent = default!; 
        public UserControl CurrentContent
        {
            get => _currentContent;
            set => SetProperty(ref _currentContent, value.ThrowIfNull(nameof(value)));
        }

        // Initializes throught property (in ChangeScene which called in ctor).
        private SceneItem _selectedSceneItem = default!;
        public SceneItem SelectedSceneItem
        {
            get => _selectedSceneItem;
            set
            {
                SetProperty(ref _selectedSceneItem, value.ThrowIfNull(nameof(value)));
                CurrentContent = value.Content;
            }
        }

        private IAsyncCommand<ThingPerformerInfo> SubmitThings { get; }

        private IAsyncCommand<ContentDirectoryParametersInfo> SubmitContents { get; }

        public ICommand AppCloseCommand { get; }

        public ICommand ReturnToStartViewCommand { get; }

        public ICommand ForceReturnToStartViewCommand { get; }

        public IReadOnlyList<SceneItem> SceneItems => _scenes.SceneItems;


        public MainWindowViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator.ThrowIfNull(nameof(eventAggregator));

            _eventAggregator
                .GetEvent<AppraiseInputThingsMessage>()
                .Subscribe(SendRequestToService);

            _eventAggregator
                .GetEvent<AppraiseLocalThingsFileMessage>()
                .Subscribe(SendRequestToService);

            _eventAggregator
                .GetEvent<AppraiseGoogleDriveThingsFileMessage>()
                .Subscribe(SendRequestToService);

            _eventAggregator
                .GetEvent<ProcessContentDirectoryMessage>()
                .Subscribe(ProcessContentDirectory);

            _title = DesktopOptions.Title;
            _isNotBusy = true;
            _scenes = new SceneItemsCollection();

            SubmitThings = new AsyncRelayCommand<ThingPerformerInfo>(
                SubmitThingsAsync, info => IsNotBusy
            );
            SubmitContents = new AsyncRelayCommand<ContentDirectoryParametersInfo>(
                SubmitContentsAsync, parameters => IsNotBusy
            );

            AppCloseCommand = new DelegateCommand(
                ApplicationCloseCommand.Execute, ApplicationCloseCommand.CanExecute
            );
            ReturnToStartViewCommand = new DelegateCommand<UserControl>(
                ReturnToStartView, CanReturnToStartView
            );
            ForceReturnToStartViewCommand = new DelegateCommand<UserControl>(ReturnToStartView);

            // TODO: create new scenes to set views dynamically in separate tabs.
            _scenes.AddScene(
                DesktopOptions.PageNames.StartPage,
                new StartView(eventAggregator)
            );

            _scenes.AddScene(
                DesktopOptions.PageNames.TmdbPage,
                new BrowsingView(new BrowsingViewModel(new ThingSupplier(new ThingGrader())))
            );

            _scenes.AddScene(
                DesktopOptions.PageNames.OmdbPage,
                new BrowsingView(new BrowsingViewModel(new ThingSupplier(new ThingGrader())))
            );

            _scenes.AddScene(
                DesktopOptions.PageNames.SteamPage,
                new BrowsingView(new BrowsingViewModel(new ThingSupplier(new ThingGrader())))
            );

            _scenes.AddScene(
                DesktopOptions.PageNames.ExpertModePage,
                new ProgressView()
            );

            _scenes.AddScene(
                DesktopOptions.PageNames.ToplistEditorPage,
                new ToplistView()
            );

            _scenes.AddScene(
                DesktopOptions.PageNames.ContentDirectoriesPage,
                new ContentDirectoriesView()
            );

            ChangeScene(DesktopOptions.PageNames.StartPage);
        }

        private string GetServiceNameFromStartControl()
        {
            var startControl = _scenes.GetDataContext<StartViewModel>(
                DesktopOptions.PageNames.StartPage
            );
            return startControl.SelectedService;
        }

        private void ChangeScene(string controlIdentifier)
        {
            SelectedSceneItem = _scenes.GetSceneItem(controlIdentifier);
        }


        private void ReturnToStartView(UserControl currentContent)
        {
            ChangeScene(DesktopOptions.PageNames.StartPage);
        }

        private bool CanReturnToStartView(UserControl currentContent)
        {
            return !(currentContent is StartView) && IsNotBusy;
        }

        // TODO: try to move this logic in regions into new classes.

        #region Things Processing

        private void SendRequestToService(ThingsDataToAppraise thingsData)
        {
            thingsData.ThrowIfNull(nameof(thingsData));

            string message = $"Selected storage name: {thingsData.StorageName}, " +
                             $"Selected data source: {thingsData.DataSource.ToString()}";
            Console.WriteLine(message);
            _logger.Debug(message);

            string serviceName = GetServiceNameFromStartControl();

            var thingsInfo = new ThingPerformerInfo(serviceName, thingsData);

            CurrentContent = new ProgressView();
            SubmitThings.ExecuteAsync(thingsInfo);
        }

        private async Task SubmitThingsAsync(ThingPerformerInfo thingsInfo)
        {
            try
            {
                IsNotBusy = false;

                var thingPerformer = new ThingPerformer();
                ThingResultInfo result = await thingPerformer.PerformAsync(thingsInfo);
                ProcessStatusOperation(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception occurred during data processing request.");
                MessageBoxProvider.ShowError(ex.Message);

                ForceReturnToStartViewCommand.Execute(null);
            }
            finally
            {
                IsNotBusy = true;
            }
        }

        private void ProcessStatusOperation(ThingResultInfo result)
        {
            result.ThrowIfNull(nameof(result));

            if (result.Response?.Metadata.ResultStatus == ServiceStatus.Ok)
            {
                ChangeSceneAndUpdateItems(result.ServiceName, result.Response);
            }
            else
            {
                MessageBoxProvider.ShowError("Request to ThingAppraiser service failed.");
                ForceReturnToStartViewCommand.Execute(null);
            }
        }

        private void ChangeSceneAndUpdateItems(string controlIdentifier,
            ProcessingResponse response)
        {
            try
            {
                SceneItem sceneItem = _scenes.GetSceneItem(controlIdentifier);
                if (sceneItem.Content.DataContext is BrowsingViewModel controlViewModel)
                {
                    controlViewModel.Update(response);
                    SelectedSceneItem = sceneItem;
                }
                else
                {
                    throw new InvalidOperationException(
                        $"Cannot find scene \"{controlIdentifier}\" to update."
                    );
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception occurred during updating page with things items.");
                MessageBoxProvider.ShowError(ex.Message);
            }
        }

        #endregion

        #region Content Directory Processing

        private void ProcessContentDirectory(ContentDirectoryParametersInfo parameters)
        {
            parameters.ThrowIfNull(nameof(parameters));

            string message = $"Selected directory path: {parameters.DirectoryPath}, " +
                             $"Selected content type: {parameters.ContentType.ToString()}";
            Console.WriteLine(message);
            _logger.Debug(message);

            CurrentContent = new ProgressView();
            SubmitContents.ExecuteAsync(parameters);
        }

        private async Task SubmitContentsAsync(ContentDirectoryParametersInfo parameters)
        {
            try
            {
                IsNotBusy = false;

                var contentDirectoryPerformer = new ContentDirectoryPerformer();
                ContentDirectoryInfo result =
                    await contentDirectoryPerformer.PerformAsync(parameters);
                ProcessContentDirectoryResults(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception occurred during content directory processing request.");
                MessageBoxProvider.ShowError(ex.Message);

                ForceReturnToStartViewCommand.Execute(null);
            }
            finally
            {
                IsNotBusy = true;
            }
        }

        private void ProcessContentDirectoryResults(ContentDirectoryInfo result)
        {
            result.ThrowIfNull(nameof(result));

            _eventAggregator
                .GetEvent<UpdateContentDirectoryInfoMessage>()
                .Publish(result);

            SetContentDirectoryView();
        }

        private void SetContentDirectoryView()
        {
            try
            {
                SceneItem sceneItem =
                    _scenes.GetSceneItem(DesktopOptions.PageNames.ContentDirectoriesPage);
                if (sceneItem.Content is ContentDirectoriesView contentDirectoriesView)
                {
                    SelectedSceneItem = sceneItem;
                }
                else
                {
                    throw new InvalidOperationException(
                        $"Cannot find scene \"{DesktopOptions.PageNames.ContentDirectoriesPage}\"" +
                        " to update."
                    );
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception occurred during updating content directory page.");
                MessageBoxProvider.ShowError(ex.Message);
            }
        }

        #endregion
    }
}
