using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ThingAppraiser.Data;
using ThingAppraiser.Data.Models;
using ThingAppraiser.DesktopApp.Domain;
using ThingAppraiser.DesktopApp.Domain.Commands;
using ThingAppraiser.DesktopApp.Models;
using ThingAppraiser.DesktopApp.Models.DataProducers;
using ThingAppraiser.DesktopApp.Models.DataSuppliers;
using ThingAppraiser.DesktopApp.Views;
using ThingAppraiser.Core.Building;
using ThingAppraiser.IO.Input;
using ThingAppraiser.Logging;

namespace ThingAppraiser.DesktopApp.ViewModels
{
    internal class MainWindowViewModel : ViewModelBase
    {
        private static readonly LoggerAbstraction _logger =
            LoggerAbstraction.CreateLoggerInstanceFor<MainWindowViewModel>();

        private readonly IRequirementsCreator _requirementsCreator = new RequirementsCreator();

        private readonly ServiceProxy _serviceProxy = new ServiceProxy();

        private readonly Dictionary<string, int> _sceneIdentifiers;

        private bool _isNotBusy = true;

        private UserControl _currentContent;

        private string _selectedStorageName;

        private DataSource _selectedDataSource = DataSource.Nothing;

        private SceneItem _selectedSceneItem;

        private ThingProducer _thingProducer;

        public bool IsNotBusy
        {
            get => _isNotBusy;
            private set => SetProperty(ref _isNotBusy, value);
        }

        public UserControl CurrentContent
        {
            get => _currentContent;
            set => SetProperty(ref _currentContent, value);
        }

        public string SelectedStorageName
        {
            get => _selectedStorageName;
            private set => SetProperty(ref _selectedStorageName, value);
        }

        public DataSource SelectedDataSource
        {
            get => _selectedDataSource;
            private set => SetProperty(ref _selectedDataSource, value);
        }

        public SceneItem SelectedSceneItem
        {
            get => _selectedSceneItem;
            set
            {
                SetProperty(ref _selectedSceneItem, value);
                CurrentContent = value.Content;
            }
        }

        public IAsyncCommand<DataSource> Submit { get; }

        public ICommand AppCloseCommand => new RelayCommand(ApplicationCloseCommand.Execute,
                                                            ApplicationCloseCommand.CanExecute);

        public ICommand ReturnToStartViewCommand => new RelayCommand(ReturnToStartView,
                                                                     CanReturnToStartView);

        public ICommand ForceReturnToStartViewCommand => new RelayCommand(ReturnToStartView);

        public SceneItem[] SceneItems { get; }

        public object DialogIdentifier { get; }


        public MainWindowViewModel(object dialogIdentifier)
        {
            Submit = new AsyncRelayCommand<DataSource>(ExecuteSubmitAsync, CanExecuteSubmit,
                                                       new CommonErrorHandler());

            _sceneIdentifiers = new Dictionary<string, int>
            {
                { DesktopOptions.PageNames.StartPage, 0 },
                { DesktopOptions.PageNames.TmdbPage, 1 },
                { DesktopOptions.PageNames.OmdbPage, 2 },
                { DesktopOptions.PageNames.SteamPage, 3 },
                { DesktopOptions.PageNames.ExpertModePage, 4 },
                { DesktopOptions.PageNames.ToplistStartPage, 5 },
                { DesktopOptions.PageNames.ToplistEditorPage, 6 }
            };

            SceneItems = new[]
            {
                new SceneItem(
                    DesktopOptions.PageNames.StartPage,
                    new StartControl(dialogIdentifier)
                ),

                new SceneItem(
                    DesktopOptions.PageNames.TmdbPage,
                    new BrowsingControl(
                        new BrowsingViewModel(new ThingSupplier(new ThingGrader()))
                    )
                ),

                new SceneItem(
                    DesktopOptions.PageNames.OmdbPage,
                    new BrowsingControl(
                        new BrowsingViewModel(new ThingSupplier(new ThingGrader()))
                    )
                ),

                new SceneItem(
                    DesktopOptions.PageNames.SteamPage,
                    new BrowsingControl(
                        new BrowsingViewModel(new ThingSupplier(new ThingGrader()))
                    )
                ),

                new SceneItem(DesktopOptions.PageNames.ExpertModePage, new ProgressDialog()),

                new SceneItem(
                    DesktopOptions.PageNames.ToplistStartPage,
                    new ToplistStartControl(dialogIdentifier)
                ),

                new SceneItem(
                    DesktopOptions.PageNames.ToplistEditorPage,
                    new ToplistEditorControl()
                )
            };

            ChangeScene(DesktopOptions.PageNames.StartPage);
            DialogIdentifier = dialogIdentifier;
        }

        private static void ThrowIfInvalidData(IReadOnlyCollection<string> data)
        {
            if (data.IsNullOrEmpty() || data.Count < 2)
            {
                throw new InvalidOperationException("Insufficient amount of data to be processed.");
            }
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

        public void OpenToplistEditorScene(string toplistName, string toplistType,
            string toplistFormat)
        {
            ChangeSceneAndConstructNewToplist(DesktopOptions.PageNames.ToplistEditorPage,
                                              toplistName, toplistType, toplistFormat);
        }

        private string FindServiceNameAtStartControl()
        {
            int index = _sceneIdentifiers[DesktopOptions.PageNames.StartPage];
            SceneItem sceneItem = SceneItems[index];
            if (sceneItem.Content.DataContext is StartViewModel startControl)
            {
                return startControl.SelectedService;
            }
            return string.Empty;
        }

        private void ChangeScene(string controlIdentifier)
        {
            int index = _sceneIdentifiers[controlIdentifier];
            SelectedSceneItem = SceneItems[index];
        }

        private void ChangeSceneAndUpdateItems(string controlIdentifier,
            ProcessingResponse response)
        {
            int index = _sceneIdentifiers[controlIdentifier];
            SceneItem sceneItem = SceneItems[index];
            if (sceneItem.Content.DataContext is BrowsingViewModel controlViewModel)
            {
                controlViewModel.Update(response);
                SelectedSceneItem = sceneItem;
            }
        }

        private void ChangeSceneAndConstructNewToplist(string controlIdentifier,
            string toplistName, string toplistType, string toplistFormat)
        {
            int index = _sceneIdentifiers[controlIdentifier];
            SceneItem sceneItem = SceneItems[index];
            if (sceneItem.Content.DataContext is ToplistEditorViewModel toplistEditorViewModel)
            {
                toplistEditorViewModel.ConstructNewToplist(toplistName, toplistType, toplistFormat);
                SelectedSceneItem = sceneItem;
            }
        }

        private void ExecuteSending()
        {
            string message = $"SelectedStorageName={SelectedStorageName}, " +
                             $"SelectedDataSource={SelectedDataSource}";
            Console.WriteLine(message);
            _logger.Debug(message);

            CurrentContent = new ProgressDialog();
            Submit.ExecuteAsync(SelectedDataSource);
        }

        private void ProcessStatusOperation(ProcessingResponse response)
        {
            if (response?.MetaData.ResultStatus == ServiceStatus.Ok)
            {
                string serviceName = FindServiceNameAtStartControl();
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
                ProcessingResponse response = await _serviceProxy.SendPostRequest(requestParams);
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

        private void ReturnToStartView(object obj)
        {
            ChangeScene(DesktopOptions.PageNames.StartPage);
        }

        private bool CanReturnToStartView(object obj)
        {
            return !(obj is StartControl) && IsNotBusy;
        }

        private async Task<RequestParams> ConfigureServiceRequest(DataSource dataSource)
        {
            switch (dataSource)
            {
                case DataSource.Nothing:
                    _logger.Error("Data source wasn't set.");
                    throw new InvalidOperationException("Data source wasn't set.");

                case DataSource.InputThing:
                    return await CreateRequestWithUserInputData();

                case DataSource.LocalFile:
                    return await CreateRequestWithLocalFileData();

                case DataSource.GoogleDrive:
                    return await CreateGoogleDriveRequest();

                default:
                    var ex = new ArgumentOutOfRangeException(
                        nameof(dataSource), dataSource,
                        "Couldn't recognize specified data source type."
                    );
                    _logger.Error(ex, $"Passed incorrect data to method: {dataSource}");
                    throw ex;
            }
        }

        private async Task<RequestParams> CreateRequestWithUserInputData()
        {
            CreateBasicRequirements();

            List<string> thingNames = await Task.Run(
                () => _thingProducer.ReadThingNames("Service request")
            );

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
            List<string> thingNames = await Task.Run(
                () => localFileReader.ReadThingNames(SelectedStorageName)
            );

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
                ConfigModule.GetConfigForInputter(
                    ConfigOptions.Inputters.GoogleDriveReaderSimpleName
                )
            );
            List<string> thingNames = await Task.Run(
                () => googleDriveReader.ReadThingNames(SelectedStorageName)
            );

            ThrowIfInvalidData(thingNames);
            return new RequestParams
            {
                ThingNames = thingNames,
                Requirements = _requirementsCreator.GetResult()
            };
        }

        private void CreateBasicRequirements()
        {
            string serviceName = FindServiceNameAtStartControl();
            serviceName = ConfigContract.GetProperServiceName(serviceName);

            _requirementsCreator.Reset();
            _requirementsCreator.AddServiceRequirement(serviceName);
            _requirementsCreator.AddAppraisalRequirement($"{serviceName}Common");
        }
    }
}
