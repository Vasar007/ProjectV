using System;
using ThingAppraiser.Logging;

namespace ThingAppraiser.Core.Building
{
    /// <summary>
    /// Builder class which provides the way of constructing <see cref="CShell" /> instances from
    /// default App.config file.
    /// </summary>
    /// <remarks>
    /// Structure of App.config file must satisfy certain contracts, otherwise different exception
    /// could be thrown.
    /// </remarks>
    public sealed class CShellBuilderFromConfig : IShellBuilder
    {
        /// <summary>
        /// Logger instance for current class.
        /// </summary>
        private static readonly CLoggerAbstraction s_logger =
            CLoggerAbstraction.CreateLoggerInstanceFor<CShellBuilderFromConfig>();

        /// <summary>
        /// Attribute name for message handler.
        /// </summary>
        private readonly String _messageHandlerTypeParameterName = "MessageHandlerType";

        /// <summary>
        /// Attribute name for default input filename of input manager.
        /// </summary>
        private readonly String _defaultInFilenameParameterName = "DefaultInFilename";

        /// <summary>
        /// Attribute name for number of inputters in config.
        /// </summary>
        private readonly String _inputtersNumberParameterName = "InputtersNumber";

        /// <summary>
        /// Attribute name for inputters base part of the name in config. Each inputter definition
        /// must contain this base part with serial number after it without spaces. Numbering
        /// starts with 1.
        /// </summary>
        private readonly String _inputterBaseParameterName = "Inputter";

        /// <summary>
        /// Attribute name for crawlers output flag of crawlers manager.
        /// </summary>
        private readonly String _crawlersOutputParameterName = "CrawlersOutputFlag";

        /// <summary>
        /// Attribute name for number of crawlers in config.
        /// </summary>
        private readonly String _crawlersNumberParameterName = "CrawlersNumber";

        /// <summary>
        /// Attribute name for crawler base part of the name in config. Each crawler definition
        /// must contain this base part with serial number after it without spaces. Numbering
        /// starts with 1.
        /// </summary>
        private readonly String _crawlerBaseParameterName = "Crawler";

        /// <summary>
        /// Attribute name for appraisers output flag of appraisers manager.
        /// </summary>
        private readonly String _appraisersOutputParameterName = "AppraisersOutputFlag";

        /// <summary>
        /// Attribute name for number of appraises in config.
        /// </summary>
        private readonly String _appraisersNumberParameterName = "AppraisersNumber";

        /// <summary>
        /// Attribute name for appraiser base part of the name in config. Each appraiser definition
        /// must contain this base part with serial number after it without spaces. Numbering
        /// starts with 1.
        /// </summary>
        private readonly String _appraiserBaseParameterName = "Appraiser";

        /// <summary>
        /// Attribute name for default output filename of output manager.
        /// </summary>
        private readonly String _defaultOutFilenameParameterName = "DefaultOutFilename";

        /// <summary>
        /// Attribute name for number of outputters in config.
        /// </summary>
        private readonly String _outputtersNumberParameterName = "OutputtersNumber";

        /// <summary>
        /// Attribute name for output base part of the name in config. Each output definition
        /// must contain this base part with serial number after it without spaces. Numbering
        /// starts with 1.
        /// </summary>
        private readonly String _outputterBaseParameterName = "Outputter";

        /// <summary>
        /// Attribute name for number of repositories in config.
        /// </summary>
        private readonly String _repositoriesNumberParameterName = "RepositoriesNumber";

        /// <summary>
        /// Attribute name for repository base part of the name in config. Each repository
        /// definition must contain this base part with serial number after it without spaces.
        /// Numbering starts with 1.
        /// </summary>
        private readonly String _repositoryBaseParameterName = "Repository";

        /// <summary>
        /// Attribute name for connection string for data base component.
        /// </summary>
        private readonly String _connectionStringParameterName = "ConnectionString";


        /// <summary>
        /// Variables which saves input manager instance during building process.
        /// </summary>
        private IO.Input.CInputManager _inputManager;

        /// <summary>
        /// Variables which saves crawlers manager instance during building process.
        /// </summary>
        private Crawlers.CCrawlersManager _crawlersManager;

        /// <summary>
        /// Variables which saves appraisers manager instance during building process.
        /// </summary>
        private Appraisers.CAppraisersManager _appraisersManager;

        /// <summary>
        /// Variables which saves output manager instance during building process.
        /// </summary>
        private IO.Output.COutputManager _outputManager;

        /// <summary>
        /// Variables which saves data base manager instance during building process.
        /// </summary>
        private DAL.CDataBaseManager _dataBaseManager;


        /// <summary>
        /// Initializes builder which works with default App.config file.
        /// </summary>
        public CShellBuilderFromConfig()
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
            Communication.SGlobalMessageHandler.MessageHandler =
                SServiceBuilder.CreateMessageHandlerWithConfigParameters(
                    SConfigParser.GetValueByParameterKey(_messageHandlerTypeParameterName)
                );
        }

        /// <inheritdoc />
        public void BuildInputManager()
        {
            _inputManager = new IO.Input.CInputManager(
                SConfigParser.GetValueByParameterKey(_defaultInFilenameParameterName)
            );

            var inputtersNumber = SConfigParser.GetValueByParameterKey<Int32>(
                _inputtersNumberParameterName
            );
            for (Int32 i = 1; i <= inputtersNumber; ++i)
            {
                _inputManager.Add(SServiceBuilder.CreateInputterWithConfigParameters(
                    SConfigParser.GetValueByParameterKey(_inputterBaseParameterName + i)
                ));
            }
        }

        /// <inheritdoc />
        public void BuildCrawlersManager()
        {
            _crawlersManager = new Crawlers.CCrawlersManager(
                SConfigParser.GetValueByParameterKey<Boolean>(_crawlersOutputParameterName)
            );

            var crawlersNumber = SConfigParser.GetValueByParameterKey<Int32>(
                _crawlersNumberParameterName
            );
            for (Int32 i = 1; i <= crawlersNumber; ++i)
            {
                _crawlersManager.Add(SServiceBuilder.CreateCrawlerWithConfigParameters(
                    SConfigParser.GetValueByParameterKey(_crawlerBaseParameterName + i)
                ));
            }
        }

        /// <inheritdoc />
        public void BuildAppraisersManager()
        {
            _appraisersManager = new Appraisers.CAppraisersManager(
                SConfigParser.GetValueByParameterKey<Boolean>(_appraisersOutputParameterName)
            );

            var appraisersNumber = SConfigParser.GetValueByParameterKey<Int32>(
                _appraisersNumberParameterName
            );
            for (Int32 i = 1; i <= appraisersNumber; ++i)
            {
                _appraisersManager.Add(SServiceBuilder.CreateAppraiserWithConfigParameters(
                    SConfigParser.GetValueByParameterKey(_appraiserBaseParameterName + i)
                ));
            }
        }

        /// <inheritdoc />
        public void BuildOutputManager()
        {
            _outputManager = new IO.Output.COutputManager(
                SConfigParser.GetValueByParameterKey(_defaultOutFilenameParameterName)
            );

            var outputtersNumber = SConfigParser.GetValueByParameterKey<Int32>(
                _outputtersNumberParameterName
            );
            for (Int32 i = 1; i <= outputtersNumber; ++i)
            {
                _outputManager.Add(SServiceBuilder.CreateOutputterWithConfigParameters(
                    SConfigParser.GetValueByParameterKey(_outputterBaseParameterName + i)
                ));
            }
        }

        /// <inheritdoc />
        public void BuildDataBaseManager()
        {
            var connectionString = SConfigParser.GetValueByParameterKey(
                _connectionStringParameterName
            );
            var dataBaseSettings = new DAL.CDataStorageSettings(connectionString);
            _dataBaseManager = new DAL.CDataBaseManager(
                new DAL.Repositories.CResultInfoRepository(dataBaseSettings),
                new DAL.Repositories.CRatingRepository(dataBaseSettings)
            );

            var repositoriesNumber = SConfigParser.GetValueByParameterKey<Int32>(
                _repositoriesNumberParameterName
            );
            for (Int32 i = 1; i <= repositoriesNumber; ++i)
            {
                _dataBaseManager.DataRepositoriesManager.Add(
                    SServiceBuilder.CreateRepositoryWithConfigParameters(
                        SConfigParser.GetValueByParameterKey(_repositoryBaseParameterName + i),
                        dataBaseSettings
                    )
                );
            }
        }

        /// <inheritdoc />
        public CShell GetResult()
        {
            s_logger.Info("Created Shell from App config.");
            return new CShell(_inputManager, _crawlersManager, _appraisersManager, _outputManager,
                              _dataBaseManager);
        }

        #endregion
    }
}
