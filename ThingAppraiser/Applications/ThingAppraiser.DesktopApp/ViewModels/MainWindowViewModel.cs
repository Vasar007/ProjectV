using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Prism.Events;
using Prism.Mvvm;
using ThingAppraiser.Building;
using ThingAppraiser.Building.Service;
using ThingAppraiser.Configuration;
using ThingAppraiser.DesktopApp.Domain;
using ThingAppraiser.DesktopApp.Domain.Commands;
using ThingAppraiser.DesktopApp.Domain.Messages;
using ThingAppraiser.DesktopApp.Models;
using ThingAppraiser.DesktopApp.Models.DataProducers;
using ThingAppraiser.DesktopApp.Models.DataSuppliers;
using ThingAppraiser.DesktopApp.Models.Toplists;
using ThingAppraiser.DesktopApp.Views;
using ThingAppraiser.Extensions;
using ThingAppraiser.IO.Input.File;
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

        private readonly IRequirementsCreator _requirementsCreator;

        private readonly ServiceProxy _serviceProxy;

        private readonly SceneItemsCollection _scenes;

        private ThingProducer? _thingProducer;

        private string _title = "ThingAppraiser";
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value.ThrowIfNull(nameof(value)));
        }

        private bool _isNotBusy = true;
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

        private string _selectedStorageName = string.Empty;
        public string SelectedStorageName
        {
            get => _selectedStorageName;
            private set => SetProperty(ref _selectedStorageName, value.ThrowIfNull(nameof(value)));
        }

        private DataSource _selectedDataSource = DataSource.Nothing;
        public DataSource SelectedDataSource
        {
            get => _selectedDataSource;
            private set => SetProperty(ref _selectedDataSource, value);
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

        public IAsyncCommand<DataSource> Submit =>
            new AsyncRelayCommand<DataSource>(ExecuteSubmitAsync, CanExecuteSubmit);

        public ICommand AppCloseCommand => new RelayCommand(ApplicationCloseCommand.Execute,
                                                            ApplicationCloseCommand.CanExecute);

        public ICommand ReturnToStartViewCommand =>
            new RelayCommand<UserControl>(ReturnToStartView, CanReturnToStartView);

        public ICommand ForceReturnToStartViewCommand =>
            new RelayCommand<UserControl>(ReturnToStartView);

        public IReadOnlyList<SceneItem> SceneItems => _scenes.SceneItems;

        public object DialogIdentifier { get; }


        public MainWindowViewModel(object dialogIdentifier, IEventAggregator eventAggregator)
        {
            DialogIdentifier = dialogIdentifier.ThrowIfNull(nameof(dialogIdentifier));
            _eventAggregator = eventAggregator.ThrowIfNull(nameof(eventAggregator));

            _eventAggregator.GetEvent<OpenToplistMessage>().Subscribe(OpenToplistFile);
            _eventAggregator.GetEvent<SaveToplistMessage>().Subscribe(SaveToplistToFile);

            _requirementsCreator = new RequirementsCreator();
            _serviceProxy = new ServiceProxy();
            _scenes = new SceneItemsCollection();

            // TODO: create new scenes to set views dynamically in separate tabs.
            _scenes.AddScene(
                DesktopOptions.PageNames.StartPage,
                new StartView(dialogIdentifier)
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
                DesktopOptions.PageNames.ToplistStartPage,
                new ToplistStartView(dialogIdentifier, eventAggregator)
            );

            _scenes.AddScene(
                DesktopOptions.PageNames.ToplistEditorPage,
                new ProgressView()
            );

            _scenes.AddScene(
                DesktopOptions.PageNames.ContentFinderPage,
                new ContentFinderView(dialogIdentifier)
            );

            ChangeScene(DesktopOptions.PageNames.StartPage);
        }

        public void SendRequestToService(DataSource dataSource, string storageName)
        {
            storageName.ThrowIfNullOrEmpty(nameof(storageName));

            SelectedStorageName = storageName;
            SelectedDataSource = dataSource;
            ExecuteSending();
        }

        public void SendRequestToService(DataSource dataSource, List<string> thingList)
        {
            thingList.ThrowIfNull(nameof(thingList));

            _thingProducer = new ThingProducer(thingList);

            SelectedStorageName = "User input";
            SelectedDataSource = dataSource;
            ExecuteSending();
        }

        public void OpenToplistEditorScene(string toplistName, ToplistType toplistType,
            ToplistFormat toplistFormat)
        {
            toplistName.ThrowIfNullOrEmpty(nameof(toplistName));

            ChangeSceneAndConstructNewToplist(DesktopOptions.PageNames.ToplistEditorPage,
                                              toplistName, toplistType, toplistFormat);
        }

        public void OpenToplistFile(string toplistFilename)
        {
            toplistFilename.ThrowIfNullOrEmpty(nameof(toplistFilename));

            ChangeSceneAndLoadToplist(DesktopOptions.PageNames.ToplistEditorPage, toplistFilename);
        }

        public void SaveToplistToFile(string toplistFilename)
        {
            toplistFilename.ThrowIfNullOrEmpty(nameof(toplistFilename));

            ProcessToplistSaving(toplistFilename);
        }

        private static void ThrowIfInvalidData(IReadOnlyCollection<string> data)
        {
            const int minItemsNumberToAppraise = 2;
            if (data.IsNullOrEmpty() || data.Count < minItemsNumberToAppraise)
            {
                throw new InvalidOperationException("Insufficient amount of data to be processed.");
            }
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
                    throw new InvalidOperationException("Cannot found scene to update.");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception occurred during updating items.");
                MessageBox.Show(ex.Message, "ThingAppraiser", MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        private void ChangeSceneAndConstructNewToplist(string controlIdentifier,
            string toplistName, ToplistType toplistType, ToplistFormat toplistFormat)
        {
            try
            {
                SceneItem sceneItem = _scenes.GetSceneItem(controlIdentifier);
                sceneItem.Content = new ToplistEditorView();
                if (sceneItem.Content.DataContext is ToplistEditorViewModel toplistEditorViewModel)
                {
                    toplistEditorViewModel.ConstructNewToplist(toplistName, toplistType,
                                                               toplistFormat);
                    SelectedSceneItem = sceneItem;
                }
                else
                {
                    throw new InvalidOperationException("Cannot found scene to construct toplist.");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception occurred during toplist creation.");
                MessageBox.Show(ex.Message, "ThingAppraiser", MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        private void ChangeSceneAndLoadToplist(string controlIdentifier,
            string toplistFilename)
        {
            try
            {
                SceneItem sceneItem = _scenes.GetSceneItem(controlIdentifier);
                sceneItem.Content = new ToplistEditorView();
                if (sceneItem.Content.DataContext is ToplistEditorViewModel toplistEditorViewModel)
                {
                    toplistEditorViewModel.LoadToplist(toplistFilename);
                    SelectedSceneItem = sceneItem;
                }
                else
                {
                    throw new InvalidOperationException("Cannot found scene to load toplist.");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception occurred during toplist loading.");
                MessageBox.Show(ex.Message, "ThingAppraiser", MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        private void ProcessToplistSaving(string toplistFilename)
        {
            try
            {
                if (CurrentContent.DataContext is ToplistEditorViewModel toplistEditorViewModel)
                {
                    toplistEditorViewModel.SaveToplist(toplistFilename);

                    MessageBox.Show("Toplist was saved successfully.", "ThingAppraiser",
                                    MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    throw new InvalidOperationException(
                        "Cannot found scene to process toplist saving."
                    );
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception occurred during toplist saving.");
                MessageBox.Show(ex.Message, "ThingAppraiser", MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        private void ExecuteSending()
        {
            string message = $"SelectedStorageName={SelectedStorageName}, " +
                             $"SelectedDataSource={SelectedDataSource.ToString()}";
            Console.WriteLine(message);
            _logger.Debug(message);

            CurrentContent = new ProgressView();
            Submit.ExecuteAsync(SelectedDataSource);
        }

        private void ProcessStatusOperation(ProcessingResponse? response)
        {
            if (response?.Metadata.ResultStatus == ServiceStatus.Ok)
            {
                string serviceName = GetServiceNameFromStartControl();
                ChangeSceneAndUpdateItems(serviceName, response);
            }
            else
            {
                MessageBox.Show("Request to ThingAppraiser service failed.", "ThingAppraiser",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                ForceReturnToStartViewCommand.Execute(null);
            }
        }

        private async Task ExecuteSubmitAsync(DataSource dataSource)
        {
            try
            {
                IsNotBusy = false;
                RequestParams requestParams = await ConfigureServiceRequest(dataSource);
                ProcessingResponse? response = await _serviceProxy.SendPostRequest(requestParams);
                ProcessStatusOperation(response);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception occurred during data processing request.");
                MessageBox.Show(ex.Message, "ThingAppraiser", MessageBoxButton.OK,
                                MessageBoxImage.Error);
                ForceReturnToStartViewCommand.Execute(null);
            }
            finally
            {
                IsNotBusy = true;
            }
        }

        private bool CanExecuteSubmit(DataSource dataSource)
        {
            return IsNotBusy;
        }

        private void ReturnToStartView(UserControl currentContent)
        {
            ChangeScene(DesktopOptions.PageNames.StartPage);
        }

        private bool CanReturnToStartView(UserControl currentContent)
        {
            return !(currentContent is StartView) && IsNotBusy;
        }

        private async Task<RequestParams> ConfigureServiceRequest(DataSource dataSource)
        {
            return dataSource switch
            {
                DataSource.Nothing => throw new InvalidOperationException(
                                          "Data source was not set."
                                      ),

                DataSource.InputThing => await CreateRequestWithUserInputData(),

                DataSource.LocalFile => await CreateRequestWithLocalFileData(),

                DataSource.GoogleDrive => await CreateGoogleDriveRequest(),

                _ => throw new ArgumentOutOfRangeException(
                         nameof(dataSource), dataSource,
                         "Could not recognize specified data source type."
                     )
            };
        }

        private async Task<RequestParams> CreateRequestWithUserInputData()
        {
            CreateBasicRequirements();

            if (_thingProducer is null)
            {
                throw new InvalidOperationException(
                    $"Thing producer ({nameof(_thingProducer)}) should be initialized at first."
                );
            }

            IReadOnlyList<string> thingNames = await Task
                .Run(() => _thingProducer.ReadThingNames("Service request"))
                .ConfigureAwait(continueOnCapturedContext: false);

            ThrowIfInvalidData(thingNames);
            return new RequestParams
            {
                ThingNames = thingNames,
                Requirements = _requirementsCreator.GetResult()
            };
        }

        private async Task<RequestParams> CreateRequestWithLocalFileData()
        {
            CreateBasicRequirements();

            var localFileReader = new LocalFileReader(new SimpleFileReader());
            IReadOnlyList<string> thingNames = await Task
                .Run(() => localFileReader.ReadThingNames(SelectedStorageName))
                .ConfigureAwait(continueOnCapturedContext: false);

            ThrowIfInvalidData(thingNames);
            return new RequestParams
            {
                ThingNames = thingNames,
                Requirements = _requirementsCreator.GetResult()
            };
        }

        private async Task<RequestParams> CreateGoogleDriveRequest()
        {
            CreateBasicRequirements();

            var serviceBuilder = new ServiceBuilderForXmlConfig();
            var googleDriveReader = serviceBuilder.CreateInputter(
                ConfigModule.GetConfigForInputter(ConfigNames.Inputters.GoogleDriveReaderSimpleName)
            );

            IReadOnlyList<string> thingNames = await Task
                .Run(() => googleDriveReader.ReadThingNames(SelectedStorageName))
                .ConfigureAwait(continueOnCapturedContext: false);

            ThrowIfInvalidData(thingNames);
            return new RequestParams
            {
                ThingNames = thingNames,
                Requirements = _requirementsCreator.GetResult()
            };
        }

        private void CreateBasicRequirements()
        {
            string serviceName = GetServiceNameFromStartControl();
            serviceName = ConfigContract.GetProperServiceName(serviceName);

            _requirementsCreator.Reset();
            _requirementsCreator.AddServiceRequirement(serviceName);
            _requirementsCreator.AddAppraisalRequirement($"{serviceName}Common");
        }
    }
}
