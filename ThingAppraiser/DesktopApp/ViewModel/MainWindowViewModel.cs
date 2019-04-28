using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Linq;
using DesktopApp.Domain;
using DesktopApp.Domain.Commands;
using DesktopApp.Model.DataProducers;
using DesktopApp.Model.DataSuppliers;
using DesktopApp.View;
using ThingAppraiser;
using ThingAppraiser.Core;
using ThingAppraiser.Core.Building;
using ThingAppraiser.Crawlers;
using ThingAppraiser.Logging;

namespace DesktopApp.ViewModel
{
    public class CMainWindowViewModel : CViewModelBase
    {
        private static readonly CLoggerAbstraction s_logger =
            CLoggerAbstraction.CreateLoggerInstanceFor<CMainWindowViewModel>();

        private Boolean _isBusy;

        private UserControl _currentContent;

        private String _selectedStorageName;

        private EDataSource _selectedDataSource = EDataSource.Nothing;

        private readonly CXMLConfigCreator _xmlConfigCreator = new CXMLConfigCreator();

        private CShell _shell;

        private CThingProducer _thingProducer;

        private readonly CThingSupplier _thingSupplier = 
            new CThingSupplier(new CImageSupplierTMDB(SServiceConfigurationTMDB.Configuration));

        public Boolean IsBusy
        {
            get => _isBusy;
            private set => SetProperty(ref _isBusy, value);
        }

        public IAsyncCommand<EDataSource> Submit { get; private set; }

        public UserControl CurrentContent
        {
            get => _currentContent;
            set => SetProperty(ref _currentContent, value);
        }

        public String SelectedStorageName
        {
            get => _selectedStorageName;
            set => SetProperty(ref _selectedStorageName, value);
        }
        public EDataSource SelectedDataSource
        {
            get => _selectedDataSource;
            set
            {
                SetProperty(ref _selectedDataSource, value);
                ExecuteThingAppraiserService();
            }
        }

        public ICommand ApplicationCloseCommand => new CRelayCommand(
            SApplicationCloseCommand.Execute,
            SApplicationCloseCommand.CanExecute
        );

        public ICommand ReturnToStartViewCommand => new CRelayCommand(ReturnToStartView,
                                                                      CanReturnToStartView);


        public CMainWindowViewModel(UserControl currentContent)
        {
            CurrentContent = currentContent;
            Submit = new CAsyncRelayCommand<EDataSource>(ExecuteSubmitAsync, CanExecuteSubmit,
                                                         new CommonErrorHandler());
        }

        public void SetDataSourceAndParameters(EDataSource dataSource, String storageName)
        {
            storageName.ThrowIfNullOrEmpty(nameof(storageName));

            SelectedStorageName = storageName;
            SelectedDataSource = dataSource;
        }

        public void SetDataSourceAndParameters(EDataSource dataSource, List<String> thingList)
        {
            thingList.ThrowIfNull(nameof(thingList));

            _thingProducer = new CThingProducer(thingList);

            SelectedStorageName = "UserInput";
            SelectedDataSource = dataSource;
        }

        private void ExecuteThingAppraiserService()
        {
            Console.WriteLine($@"SelectedStorageName={SelectedStorageName}, " +
                              $@"SelectedDataSource={SelectedDataSource}");
            Submit.ExecuteAsync(SelectedDataSource);
            CurrentContent = new CProgressDialog();
        }

        private void ProcessStatusOperation(EStatus status)
        {
            if (status == EStatus.Ok)
            {
                CurrentContent = new CBrowsingControl
                {
                    // Use "new CThingSupplierMock()" for local tests without ThingAppraiser lib.
                    DataContext = new CBrowsingControlViewModel(_thingSupplier)
                };
            }
            else
            {
                ReturnToStartViewCommand.Execute(null);
            }
        }

        private async Task ExecuteSubmitAsync(EDataSource dataSource)
        {
            try
            {
                IsBusy = true;
                ConfigureShell(dataSource);
                EStatus status = await Task.Run(() => _shell.Run(SelectedStorageName));
                ProcessStatusOperation(status);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private Boolean CanExecuteSubmit(EDataSource dataSource)
        {
            return !IsBusy;
        }

        private void ReturnToStartView(Object obj)
        {
            CurrentContent = new CStartControl();
        }

        private Boolean CanReturnToStartView(Object obj)
        {
            return !(obj is CStartControl) && !IsBusy;
        }

        private void ConfigureShell(EDataSource dataSource)
        {
            switch (dataSource)
            {
                case EDataSource.Nothing:
                    s_logger.Warn("Data source wasn't set.");
                    break;

                case EDataSource.InputThing:
                    BuildShellForUserInput();
                    break;

                case EDataSource.LocalFile:
                    EnableLocalFileHandlers();
                    break;

                case EDataSource.GoogleDrive:
                    EnableGoogleDriveHandlers();
                    break;

                default:
                    var ex = new ArgumentOutOfRangeException(
                        nameof(dataSource), dataSource,
                        @"Couldn't recognize specified data source type."
                    );
                    s_logger.Error(ex, $"Passed incorrect data to method: {dataSource}");
                    throw ex;
            }
        }

        private void BuildShellForUserInput()
        {
            CreateBasicXMLConfig();

            CShell.ShellBuilderDirector.ChangeShellBuilder(
                new CShellBuilderFromXDocument(_xmlConfigCreator.GetResult())
            );
            _shell = CShell.ShellBuilderDirector.MakeShell();
            _shell.InputManager.Add(_thingProducer);
            _shell.OutputManager.Add(_thingSupplier);
        }

        private void BuildShell(XDocument config)
        {
            CShell.ShellBuilderDirector.ChangeShellBuilder(new CShellBuilderFromXDocument(config));
            _shell = CShell.ShellBuilderDirector.MakeShell();
            _shell.OutputManager.Add(_thingSupplier);
        }

        private void EnableLocalFileHandlers()
        {
            CreateBasicXMLConfig();

            _xmlConfigCreator.AddInputterElement(
                new XElement("LocalFile",
                    new XAttribute("FileReaderLocalFile", "Simple")
                )
            );
            _xmlConfigCreator.AddOutputterElement(new XElement("LocalFile"));

            BuildShell(_xmlConfigCreator.GetResult());
        }

        private void EnableGoogleDriveHandlers()
        {
            CreateBasicXMLConfig();

            _xmlConfigCreator.AddInputterElement(
                new XElement("GoogleDrive",
                    new XAttribute("FileReaderGoogleDrive", "Simple")
                )
            );
            _xmlConfigCreator.AddOutputterElement(new XElement("GoogleDrive"));

            BuildShell(_xmlConfigCreator.GetResult());
        }

        private void CreateBasicXMLConfig()
        {
            _xmlConfigCreator.Reset();

            _xmlConfigCreator.AddMessageHandlerType("ConsoleMessageHandler");
            _xmlConfigCreator.AddMessageHandlerAttribute(
                new XAttribute("ConsoleMessageHandlerSetUnicode", "false")
            );

            _xmlConfigCreator.AddInputManagerAttribute(
                new XAttribute("DefaultInFilename", "thing_names.txt")
            );

            _xmlConfigCreator.AddCrawlersManagerAttribute(
                new XAttribute("CrawlersOutputFlag", "false")
            );
            _xmlConfigCreator.AddCrawlerElement(
                new XElement("CrawlerTMDB",
                    new XAttribute("APIKeyTMDB", "f7440a70737103fea00fb6e8352a3533"),
                    new XAttribute("SearchUrlTMDB", "https://api.themoviedb.org/3/search/movie"),
                    new XAttribute("ConfigurationUrlTMDB",
                                   "https://api.themoviedb.org/3/configuration"),
                    new XAttribute("RequestsPerTimeTMDB", "30"),
                    new XAttribute("GoodStatusCodeTMDB", "200"),
                    new XAttribute("LimitAttemptsTMDB", "10"),
                    new XAttribute("MillisecondsTimeoutTMDB", "1000")
                )
            );

            _xmlConfigCreator.AddAppraisersManagerAttribute(
                new XAttribute("AppraisersOutputFlag", "false")
            );
            _xmlConfigCreator.AddAppraiserElement(
                new XElement("AppraiserTMDB")
                //new XElement("FuzzyAppraiserTMDB")
            );

            _xmlConfigCreator.AddOutputManagerAttribute(
                new XAttribute("DefaultOutFilename", "appraised_things.csv")
            );

            _xmlConfigCreator.AddDataBaseManagerAttribute(
                new XAttribute("ConnectionString", @"Data Source=(localdb)\MSSQLLocalDB;
Initial Catalog=thing_appraiser;Integrated Security=True;Connect Timeout=30;Encrypt=False;
TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False")
            );
            _xmlConfigCreator.AddRepositoryElement(
                new XElement("MovieTMDBRepository")
            );
        }
    }
}
