using ThingAppraiser.Logging;

namespace ThingAppraiser.Core.Building
{
    /// <summary>
    /// Builder class which provides the way of constructing <see cref="Shell" /> instances from
    /// default App.config file.
    /// </summary>
    /// <remarks>
    /// Structure of App.config file must satisfy certain contracts, otherwise different exception
    /// could be thrown.
    /// </remarks>
    public sealed class ShellBuilderFromConfig : IShellBuilder
    {
        /// <summary>
        /// Logger instance for current class.
        /// </summary>
        private static readonly LoggerAbstraction _logger =
            LoggerAbstraction.CreateLoggerInstanceFor<ShellBuilderFromConfig>();

        /// <summary>
        /// Attribute name for message handler.
        /// </summary>
        private static readonly string _messageHandlerTypeParameterName = "MessageHandlerType";

        /// <summary>
        /// Attribute name for default input filename of input manager.
        /// </summary>
        private static readonly string _defaultInStorageNameParameterName = "DefaultInStorageName";

        /// <summary>
        /// Attribute name for number of inputters in config.
        /// </summary>
        private static readonly string _inputtersNumberParameterName = "InputtersNumber";

        /// <summary>
        /// Attribute name for inputters base part of the name in config. Each inputter definition
        /// must contain this base part with serial number after it without spaces. Numbering
        /// starts with 1.
        /// </summary>
        private static readonly string _inputterBaseParameterName = "Inputter";

        /// <summary>
        /// Attribute name for crawlers output flag of crawlers manager.
        /// </summary>
        private static readonly string _crawlersOutputParameterName = "CrawlersOutputFlag";

        /// <summary>
        /// Attribute name for number of crawlers in config.
        /// </summary>
        private static readonly string _crawlersNumberParameterName = "CrawlersNumber";

        /// <summary>
        /// Attribute name for crawler base part of the name in config. Each crawler definition
        /// must contain this base part with serial number after it without spaces. Numbering
        /// starts with 1.
        /// </summary>
        private static readonly string _crawlerBaseParameterName = "Crawler";

        /// <summary>
        /// Attribute name for appraisers output flag of appraisers manager.
        /// </summary>
        private static readonly string _appraisersOutputParameterName = "AppraisersOutputFlag";

        /// <summary>
        /// Attribute name for number of appraises in config.
        /// </summary>
        private static readonly string _appraisersNumberParameterName = "AppraisersNumber";

        /// <summary>
        /// Attribute name for appraiser base part of the name in config. Each appraiser definition
        /// must contain this base part with serial number after it without spaces. Numbering
        /// starts with 1.
        /// </summary>
        private static readonly string _appraiserBaseParameterName = "Appraiser";

        /// <summary>
        /// Attribute name for default output filename of output manager.
        /// </summary>
        private static readonly string _defaultOutStorageNameParameterName = "DefaultOutStorageName";

        /// <summary>
        /// Attribute name for number of outputters in config.
        /// </summary>
        private static readonly string _outputtersNumberParameterName = "OutputtersNumber";

        /// <summary>
        /// Attribute name for output base part of the name in config. Each output definition
        /// must contain this base part with serial number after it without spaces. Numbering
        /// starts with 1.
        /// </summary>
        private static readonly string _outputterBaseParameterName = "Outputter";

        /// <summary>
        /// Attribute name for number of repositories in config.
        /// </summary>
        private static readonly string _repositoriesNumberParameterName = "RepositoriesNumber";

        /// <summary>
        /// Attribute name for repository base part of the name in config. Each repository
        /// definition must contain this base part with serial number after it without spaces.
        /// Numbering starts with 1.
        /// </summary>
        private static readonly string _repositoryBaseParameterName = "Repository";

        /// <summary>
        /// Attribute name for connection string for data base component.
        /// </summary>
        private static readonly string _connectionStringParameterName = "ConnectionString";

        /// <summary>
        /// Provides methods to create instances of service classes.
        /// </summary>
        private readonly ServiceBuilderForAppConfig _serviceBuilder =
            new ServiceBuilderForAppConfig();

        /// <summary>
        /// Variables which saves input manager instance during building process.
        /// </summary>
        private IO.Input.InputManager _inputManager;

        /// <summary>
        /// Variables which saves crawlers manager instance during building process.
        /// </summary>
        private Crawlers.CrawlersManager _crawlersManager;

        /// <summary>
        /// Variables which saves appraisers manager instance during building process.
        /// </summary>
        private Appraisers.AppraisersManager _appraisersManager;

        /// <summary>
        /// Variables which saves output manager instance during building process.
        /// </summary>
        private IO.Output.OutputManager _outputManager;

        /// <summary>
        /// Variables which saves data base manager instance during building process.
        /// </summary>
        private DAL.DataBaseManager _dataBaseManager;


        /// <summary>
        /// Initializes builder which works with default App.config file.
        /// </summary>
        public ShellBuilderFromConfig()
        {
            Reset();
        }

        #region IShellBuilder Implementation

        /// <inheritdoc />
        public void Reset()
        {
            _inputManager = null;
            _crawlersManager = null;
            _appraisersManager = null;
            _outputManager = null;
            _dataBaseManager = null;
        }

        /// <inheritdoc />
        public void BuildMessageHandler()
        {
            Communication.GlobalMessageHandler.SetMessageHangler(
                _serviceBuilder.CreateMessageHandler(
                    ConfigParser.GetValueByParameterKey(_messageHandlerTypeParameterName)
                )
            );
        }

        /// <inheritdoc />
        public void BuildInputManager()
        {
            _inputManager = new IO.Input.InputManager(
                ConfigParser.GetValueByParameterKey(_defaultInStorageNameParameterName)
            );

            var inputtersNumber = ConfigParser.GetValueByParameterKey<int>(
                _inputtersNumberParameterName
            );
            for (int i = 1; i <= inputtersNumber; ++i)
            {
                _inputManager.Add(_serviceBuilder.CreateInputter(
                    ConfigParser.GetValueByParameterKey(_inputterBaseParameterName + i)
                ));
            }
        }

        /// <inheritdoc />
        public void BuildCrawlersManager()
        {
            _crawlersManager = new Crawlers.CrawlersManager(
                ConfigParser.GetValueByParameterKey<bool>(_crawlersOutputParameterName)
            );

            var crawlersNumber = ConfigParser.GetValueByParameterKey<int>(
                _crawlersNumberParameterName
            );
            for (int i = 1; i <= crawlersNumber; ++i)
            {
                _crawlersManager.Add(_serviceBuilder.CreateCrawler(
                    ConfigParser.GetValueByParameterKey(_crawlerBaseParameterName + i)
                ));
            }
        }

        /// <inheritdoc />
        public void BuildAppraisersManager()
        {
            _appraisersManager = new Appraisers.AppraisersManager(
                ConfigParser.GetValueByParameterKey<bool>(_appraisersOutputParameterName)
            );

            var appraisersNumber = ConfigParser.GetValueByParameterKey<int>(
                _appraisersNumberParameterName
            );
            for (int i = 1; i <= appraisersNumber; ++i)
            {
                _appraisersManager.Add(_serviceBuilder.CreateAppraiser(
                    ConfigParser.GetValueByParameterKey(_appraiserBaseParameterName + i)
                ));
            }
        }

        /// <inheritdoc />
        public void BuildOutputManager()
        {
            _outputManager = new IO.Output.OutputManager(
                ConfigParser.GetValueByParameterKey(_defaultOutStorageNameParameterName)
            );

            var outputtersNumber = ConfigParser.GetValueByParameterKey<int>(
                _outputtersNumberParameterName
            );
            for (int i = 1; i <= outputtersNumber; ++i)
            {
                _outputManager.Add(_serviceBuilder.CreateOutputter(
                    ConfigParser.GetValueByParameterKey(_outputterBaseParameterName + i)
                ));
            }
        }

        /// <inheritdoc />
        public void BuildDataBaseManager()
        {
            var connectionString = ConfigParser.GetValueByParameterKey(
                _connectionStringParameterName
            );
            var dataBaseSettings = new DAL.DataStorageSettings(connectionString);
            _dataBaseManager = new DAL.DataBaseManager(
                new DAL.Repositories.ResultInfoRepository(dataBaseSettings),
                new DAL.Repositories.RatingRepository(dataBaseSettings)
            );

            var repositoriesNumber = ConfigParser.GetValueByParameterKey<int>(
                _repositoriesNumberParameterName
            );
            for (int i = 1; i <= repositoriesNumber; ++i)
            {
                _dataBaseManager.DataRepositoriesManager.Add(
                    _serviceBuilder.CreateRepository(
                        ConfigParser.GetValueByParameterKey(_repositoryBaseParameterName + i),
                        dataBaseSettings
                    )
                );
            }
        }

        /// <inheritdoc />
        public Shell GetResult()
        {
            _logger.Info("Created Shell from App config.");
            return new Shell(_inputManager, _crawlersManager, _appraisersManager, _outputManager,
                             _dataBaseManager);
        }

        #endregion
    }
}
