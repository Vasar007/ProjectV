using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ThingAppraiser.DesktopApp.Domain;
using ThingAppraiser.DesktopApp.Domain.Commands;
using ThingAppraiser.DesktopApp.Models.DataProducers;
using ThingAppraiser.DesktopApp.Models.DataSuppliers;
using ThingAppraiser.DesktopApp.Views;
using ThingAppraiser.Core.Building;
using ThingAppraiser.Crawlers;
using ThingAppraiser.Logging;
using ThingAppraiser.Data;
using ThingAppraiser.IO.Input;
using ThingAppraiser.Data.Models;
using ThingAppraiser.Data.Crawlers;

namespace ThingAppraiser.DesktopApp.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private static readonly LoggerAbstraction _logger =
            LoggerAbstraction.CreateLoggerInstanceFor<MainWindowViewModel>();

        private bool _isBusy;

        private UserControl _currentContent;

        private string _selectedStorageName;

        private DataSource _selectedDataSource = DataSource.Nothing;

        private ThingProducer _thingProducer;

        private readonly ThingSupplier _thingSupplier = 
            new ThingSupplier(new TmdbImageSupplier(TmdbServiceConfiguration.Configuration));

        private readonly IRequirementsCreator _requirementsCreator = new RequirementsCreator();

        private readonly ServiceProxy _serviceProxy = new ServiceProxy();

        public bool IsBusy
        {
            get => _isBusy;
            private set => SetProperty(ref _isBusy, value);
        }

        public IAsyncCommand<DataSource> Submit { get; private set; }

        public UserControl CurrentContent
        {
            get => _currentContent;
            set => SetProperty(ref _currentContent, value);
        }

        public string SelectedStorageName
        {
            get => _selectedStorageName;
            set => SetProperty(ref _selectedStorageName, value);
        }
        public DataSource SelectedDataSource
        {
            get => _selectedDataSource;
            set
            {
                SetProperty(ref _selectedDataSource, value);
                ExecuteThingAppraiserService();
            }
        }

        public ICommand AppCloseCommand => new RelayCommand(ApplicationCloseCommand.Execute,
                                                            ApplicationCloseCommand.CanExecute);

        public ICommand ReturnToStartViewCommand => new RelayCommand(ReturnToStartView,
                                                                     CanReturnToStartView);

        public ICommand ForceReturnToStartViewCommand => new RelayCommand(ReturnToStartView);


        public MainWindowViewModel(UserControl currentContent)
        {
            CurrentContent = currentContent;
            Submit = new AsyncRelayCommand<DataSource>(ExecuteSubmitAsync, CanExecuteSubmit,
                                                       new CommonErrorHandler());
        }

        private static void ThrowIfInvalidData(List<string> data)
        {
            if (data.IsNullOrEmpty() || data.Count < 2)
            {
                throw new InvalidOperationException("Insufficient amount of data to be processed.");
            }
        }

        public void SetDataSourceAndParameters(DataSource dataSource, string storageName)
        {
            storageName.ThrowIfNullOrEmpty(nameof(storageName));

            SelectedStorageName = storageName;
            SelectedDataSource = dataSource;
        }

        public void SetDataSourceAndParameters(DataSource dataSource, List<string> thingList)
        {
            thingList.ThrowIfNull(nameof(thingList));

            _thingProducer = new ThingProducer(thingList);

            SelectedStorageName = "UserInput";
            SelectedDataSource = dataSource;
        }

        private void ExecuteThingAppraiserService()
        {
            Console.WriteLine($@"SelectedStorageName={SelectedStorageName}, " +
                              $@"SelectedDataSource={SelectedDataSource}");
            CurrentContent = new ProgressDialog();
            Submit.ExecuteAsync(SelectedDataSource);
        }

        private void ProcessStatusOperation(ProcessingResponse response)
        {
            if (response?.MetaData.ResultStatus == ServiceStatus.Ok)
            {
                if (!TmdbServiceConfiguration.HasValue())
                {
                    var serviceConfig = (TmdbServiceConfigurationInfo)
                        response.MetaData.OptionalData[nameof(TmdbServiceConfiguration)];
                    TmdbServiceConfiguration.SetServiceConfigurationIfNeed(serviceConfig);
                }
                
                _thingSupplier.SaveResults(response.RatingDataContainers, "Service response");

                CurrentContent = new BrowsingControl
                {
                    DataContext = new BrowsingControlViewModel(_thingSupplier)
                };
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
                IsBusy = true;
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
                IsBusy = false;
            }
        }

        private bool CanExecuteSubmit(DataSource dataSource)
        {
            return !IsBusy;
        }

        private void ReturnToStartView(object obj)
        {
            CurrentContent = new StartControl();
        }

        private bool CanReturnToStartView(object obj)
        {
            return !(obj is StartControl) && !IsBusy;
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
                        @"Couldn't recognize specified data source type."
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
                ConfigModule.GetConfigForInputter("GoogleDriveReaderSimple")
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
            _requirementsCreator.Reset();

            _requirementsCreator.AddServiceRequirement("Tmdb");

            _requirementsCreator.AddAppraisalRequirement("TmdbCommon");
        }
    }
}
