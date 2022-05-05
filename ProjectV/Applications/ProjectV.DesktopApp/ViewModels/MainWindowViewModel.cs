using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using Acolyte.Assertions;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using ProjectV.Configuration;
using ProjectV.DesktopApp.Domain;
using ProjectV.DesktopApp.Domain.Commands;
using ProjectV.DesktopApp.Domain.Messages;
using ProjectV.DesktopApp.Models;
using ProjectV.DesktopApp.Models.ContentDirectories;
using ProjectV.DesktopApp.Models.DataSuppliers;
using ProjectV.DesktopApp.Models.Things;
using ProjectV.DesktopApp.Views;
using ProjectV.Logging;
using ProjectV.Models.Internal;
using ProjectV.Models.WebServices.Responses;

namespace ProjectV.DesktopApp.ViewModels
{
    internal sealed class MainWindowViewModel : BindableBase
    {
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor<MainWindowViewModel>();

        private readonly IEventAggregator _eventAggregator;
        private readonly IHttpClientFactory _httpClientFactory;

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

        // Initializes through property (in SelectedSceneItem when ChangeScene called in ctor).
        private UserControl _currentContent = default!;
        public UserControl CurrentContent
        {
            get => _currentContent;
            set => SetProperty(ref _currentContent, value.ThrowIfNull(nameof(value)));
        }

        // Initializes through property (in ChangeScene which called in ctor).
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

        public ICommand GoToSettingsViewCommand { get; }

        public IReadOnlyList<SceneItem> SceneItems => _scenes.SceneItems;


        public MainWindowViewModel(
            IEventAggregator eventAggregator,
            IHttpClientFactory httpClientFactory)
        {
            _eventAggregator = eventAggregator.ThrowIfNull(nameof(eventAggregator));
            _httpClientFactory = httpClientFactory.ThrowIfNull(nameof(httpClientFactory));

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
            GoToSettingsViewCommand = new DelegateCommand(GoToSettingsView);
            AddScenes(eventAggregator);

            ChangeScene(DesktopOptions.PageNames.StartPage);
        }

        private void AddScenes(IEventAggregator eventAggregator)
        {
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

            _scenes.AddScene(
                DesktopOptions.PageNames.SettingsPage,
                new SettingsView()
            );
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
            return currentContent is not StartView && IsNotBusy;
        }

        private void GoToSettingsView()
        {
            ChangeScene(DesktopOptions.PageNames.SettingsPage);
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

                using var thingPerformer = new ThingPerformer(
                    _httpClientFactory, ConfigOptions.ProjectVService
                );
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

        private void ProcessStatusOperation(ThingResultInfo info)
        {
            info.ThrowIfNull(nameof(info));

            if (info.Result.IsSuccess && info.Result.Ok?.Metadata.ResultStatus == ServiceStatus.Ok)
            {
                ChangeSceneAndUpdateItems(info.ServiceName, info.Result.Ok);
            }
            else
            {
                string? errorDetails = info.Result.Error?.ErrorMessage ?? "Unknown error";
                string errorMessage = $"Processing request to service failed: {errorDetails}.";
                MessageBoxProvider.ShowError(errorMessage);
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
