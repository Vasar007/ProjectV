using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ThingAppraiser.Building;
using ThingAppraiser.Building.Service;
using ThingAppraiser.Configuration;
using ThingAppraiser.DesktopApp.Domain;
using ThingAppraiser.DesktopApp.Domain.Commands;
using ThingAppraiser.DesktopApp.Models;
using ThingAppraiser.DesktopApp.Models.DataProducers;
using ThingAppraiser.DesktopApp.Models.DataSuppliers;
using ThingAppraiser.DesktopApp.Models.Toplists;
using ThingAppraiser.DesktopApp.Views;
using ThingAppraiser.IO.Input.File;
using ThingAppraiser.Logging;
using ThingAppraiser.Models.Internal;
using ThingAppraiser.Models.WebService;

namespace ThingAppraiser.DesktopApp.ViewModels
{
    internal sealed class MainWindowViewModel : ViewModelBase
    {
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor<MainWindowViewModel>();

        private readonly IRequirementsCreator _requirementsCreator = new RequirementsCreator();

        private readonly ServiceProxy _serviceProxy = new ServiceProxy();

        private readonly Dictionary<string, int> _sceneIdentifiers;

        private bool _isNotBusy = true;

        // Initializes throught property (in SelectedSceneItem when ChangeScene called in ctor).
        private UserControl _currentContent = default!;

        private string _selectedStorageName = string.Empty;

        private DataSource _selectedDataSource = DataSource.Nothing;

        // Initializes throught property (in ChangeScene which called in ctor).
        private SceneItem _selectedSceneItem = default!;

        private ThingProducer? _thingProducer;

        public bool IsNotBusy
        {
            get => _isNotBusy;
            private set => SetProperty(ref _isNotBusy, value);
        }

        public UserControl CurrentContent
        {
            get => _currentContent;
            set => SetProperty(ref _currentContent, value.ThrowIfNull(nameof(value)));
        }

        public string SelectedStorageName
        {
            get => _selectedStorageName;
            private set => SetProperty(ref _selectedStorageName, value.ThrowIfNull(nameof(value)));
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
                SetProperty(ref _selectedSceneItem, value.ThrowIfNull(nameof(value)));
                CurrentContent = value.Content;
            }
        }

        public IAsyncCommand<DataSource> Submit =>
            new AsyncRelayCommand<DataSource>(ExecuteSubmitAsync, CanExecuteSubmit,
                                              new CommonErrorHandler());

        public ICommand AppCloseCommand => new RelayCommand(ApplicationCloseCommand.Execute,
                                                            ApplicationCloseCommand.CanExecute);

        public ICommand ReturnToStartViewCommand =>
            new RelayCommand<UserControl>(ReturnToStartView, CanReturnToStartView);

        public ICommand ForceReturnToStartViewCommand =>
            new RelayCommand<UserControl>(ReturnToStartView);

        public SceneItem[] SceneItems { get; }

        public object DialogIdentifier { get; }


        public MainWindowViewModel(object dialogIdentifier)
        {
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

            // TODO: create new scenes for needed views dynamically in separate tabs.
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
                    new ProgressDialog()
                )
            };

            ChangeScene(DesktopOptions.PageNames.StartPage);
            DialogIdentifier = dialogIdentifier;
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

        public void OpenToplistEditorScene(string toplistFilename)
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
            if (data.IsNullOrEmpty() || data.Count < 2)
            {
                throw new InvalidOperationException("Insufficient amount of data to be processed.");
            }
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
            string toplistName, ToplistType toplistType, ToplistFormat toplistFormat)
        {
            try
            {
                int index = _sceneIdentifiers[controlIdentifier];
                SceneItem sceneItem = SceneItems[index];
                sceneItem.Content = new ToplistEditorControl();
                if (sceneItem.Content.DataContext is ToplistEditorViewModel toplistEditorViewModel)
                {
                    toplistEditorViewModel.ConstructNewToplist(toplistName, toplistType,
                                                               toplistFormat);
                    SelectedSceneItem = sceneItem;
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
                int index = _sceneIdentifiers[controlIdentifier];
                SceneItem sceneItem = SceneItems[index];
                sceneItem.Content = new ToplistEditorControl();
                if (sceneItem.Content.DataContext is ToplistEditorViewModel toplistEditorViewModel)
                {
                    toplistEditorViewModel.LoadToplist(toplistFilename);
                    SelectedSceneItem = sceneItem;
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
            if (CurrentContent.DataContext is ToplistEditorViewModel toplistEditorViewModel)
            {
                toplistEditorViewModel.SaveToplist(toplistFilename);

                MessageBox.Show("Toplist was saved successfully.", "ThingAppraiser",
                                MessageBoxButton.OK, MessageBoxImage.Information);
            }
           
        }

        private void ExecuteSending()
        {
            string message = $"SelectedStorageName={SelectedStorageName}, " +
                             $"SelectedDataSource={SelectedDataSource.ToString()}";
            Console.WriteLine(message);
            _logger.Debug(message);

            CurrentContent = new ProgressDialog();
            Submit.ExecuteAsync(SelectedDataSource);
        }

        private void ProcessStatusOperation(ProcessingResponse? response)
        {
            if (response?.Metadata.ResultStatus == ServiceStatus.Ok)
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
            return !(currentContent is StartControl) && IsNotBusy;
        }

        private async Task<RequestParams> ConfigureServiceRequest(DataSource dataSource)
        {
            return dataSource switch
            {
                DataSource.Nothing => throw new InvalidOperationException(
                                          "Data source wasn't set."
                                      ),

                DataSource.InputThing => await CreateRequestWithUserInputData(),

                DataSource.LocalFile => await CreateRequestWithLocalFileData(),

                DataSource.GoogleDrive => await CreateGoogleDriveRequest(),

                _ => throw new ArgumentOutOfRangeException(
                         nameof(dataSource), dataSource,
                         "Couldn't recognize specified data source type."
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

            IReadOnlyList<string> thingNames = await Task.Run(
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
            IReadOnlyList<string> thingNames = await Task.Run(
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
            IReadOnlyList<string> thingNames = await Task.Run(
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
