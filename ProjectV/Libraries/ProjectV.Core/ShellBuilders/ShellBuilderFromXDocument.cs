using System;
using System.Xml.Linq;
using Acolyte.Xml;
using ProjectV.Building.Service;
using ProjectV.Logging;

namespace ProjectV.Core.ShellBuilders
{
    /// <summary>
    /// Builder class which provides the way of constructing <see cref="Shell" /> instances from
    /// <see cref="XDocument" /> config.
    /// </summary>
    /// <remarks>
    /// Structure of XML config must satisfy certain contracts, otherwise different exception could
    /// be thrown.
    /// </remarks>
    public sealed class ShellBuilderFromXDocument : ShellBuilderBase, IShellBuilder
    {
        /// <summary>
        /// Logger instance for current class.
        /// </summary>
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor<ShellBuilderFromXDocument>();

        /// <summary>
        /// Provides methods to create instances of service classes.
        /// </summary>
        private readonly ServiceBuilderForXmlConfig _serviceBuilder =
            new ServiceBuilderForXmlConfig();

        /// <summary>
        /// Helper class which contains several methods to parse XML configuration.
        /// </summary>
        private readonly XDocumentParser _documentParser;

        /// <summary>
        /// Variables which saves input manager instance during building process.
        /// </summary>
        private IO.Input.InputManager? _inputManager;

        /// <summary>
        /// Variables which saves crawlers manager instance during building process.
        /// </summary>
        private Crawlers.CrawlersManager? _crawlersManager;

        /// <summary>
        /// Variables which saves appraisers manager instance during building process.
        /// </summary>
        private Appraisers.AppraisersManager? _appraisersManager;

        /// <summary>
        /// Variables which saves output manager instance during building process.
        /// </summary>
        private IO.Output.OutputManager? _outputManager;

        /// <summary>
        /// Variables which saves data base manager instance during building process.
        /// </summary>
        private DataAccessLayer.DatabaseManager? _databaseManager;


        /// <summary>
        /// Initializes builder instance and associates <see cref="XDocumentParser" /> which
        /// provides deferred parsing of XML configuration.
        /// </summary>
        /// <param name="configuration">XML configuration of <see cref="Shell" /> class.</param>
        public ShellBuilderFromXDocument(XDocument configuration)
        {
            _documentParser = new XDocumentParser(
                new XDocument(configuration.Root.Element(_rootElementName))
            );
        }

        #region IShellBuilder Implementation

        /// <inheritdoc />
        public void Reset()
        {
            _inputManager = null;
            _crawlersManager = null;
            _appraisersManager = null;
            _outputManager = null;
            _databaseManager = null;
        }

        /// <inheritdoc />
        public void BuildMessageHandler()
        {
            XElement? messageHandlerElement = _documentParser.FindElement(
                _messageHandlerParameterName
            );
            if (messageHandlerElement is null)
            {
                throw new InvalidOperationException(
                    $"XML document has not value for {_messageHandlerParameterName}."
                );
            }

            Communication.GlobalMessageHandler.SetMessageHangler(
                _serviceBuilder.CreateMessageHandler(messageHandlerElement)
            );
        }

        /// <inheritdoc />
        /// <exception cref="InvalidOperationException">
        /// XML configuration doesn't contain element for input manager with specified name.
        /// </exception>
        public void BuildInputManager()
        {
            XElement? inputManagerElement = _documentParser.FindElement(_inputManagerParameterName);
            if (inputManagerElement is null)
            {
                throw new InvalidOperationException(
                    $"XML document has not value for {_inputManagerParameterName}."
                );
            }

            string defaultStorageName = XDocumentParser.GetAttributeValue(
                inputManagerElement, _defaultInStorageNameParameterName
            );
            _inputManager = new IO.Input.InputManager(defaultStorageName);

            foreach (XElement element in inputManagerElement.Elements())
            {
                IO.Input.IInputter inputter = _serviceBuilder.CreateInputter(element);
                _inputManager.Add(inputter);
            }
        }

        /// <inheritdoc />
        /// <exception cref="InvalidOperationException">
        /// XML configuration doesn't contain element for crawlers manager with specified name.
        /// </exception>
        public void BuildCrawlersManager()
        {
            XElement? crawlerManagerElement = _documentParser.FindElement(
                _crawlersManagerParameterName
            );
            if (crawlerManagerElement is null)
            {
                throw new InvalidOperationException(
                    $"XML document has not value for {_crawlersManagerParameterName}."
                );
            }

            var crawlersOutput = XDocumentParser.GetAttributeValue<bool>(
                crawlerManagerElement, _crawlersOutputParameterName
            );
            _crawlersManager = new Crawlers.CrawlersManager(crawlersOutput);

            foreach (XElement element in crawlerManagerElement.Elements())
            {
                Crawlers.ICrawler crawler = _serviceBuilder.CreateCrawler(element);
                _crawlersManager.Add(crawler);
            }
        }

        /// <inheritdoc />
        /// <exception cref="InvalidOperationException">
        /// XML configuration doesn't contain element for appraisers manager with specified name.
        /// </exception>
        public void BuildAppraisersManager()
        {
            XElement? appraiserManagerElement = _documentParser.FindElement(
                _appraisersManagerParameterName
            );
            if (appraiserManagerElement is null)
            {
                throw new InvalidOperationException(
                    $"XML document has not value for {_appraisersManagerParameterName}."
                );
            }

            var appraisersOutput = XDocumentParser.GetAttributeValue<bool>(
                appraiserManagerElement, _appraisersOutputParameterName
            );
            _appraisersManager = new Appraisers.AppraisersManager(appraisersOutput);

            foreach (XElement element in appraiserManagerElement.Elements())
            {
                Appraisers.IAppraiser crawler = _serviceBuilder.CreateAppraiser(element);
                _appraisersManager.Add(crawler);
            }
        }

        /// <inheritdoc />
        /// <exception cref="InvalidOperationException">
        /// XML configuration doesn't contain element for output manager with specified name.
        /// </exception>
        public void BuildOutputManager()
        {
            XElement? outputManagerElement = _documentParser.FindElement(
                _outputManagerParameterName
            );
            if (outputManagerElement is null)
            {
                throw new InvalidOperationException(
                    $"XML document has not value for {_outputManagerParameterName}."
                );
            }

            string defaultStorageName = XDocumentParser.GetAttributeValue(
                outputManagerElement, _defaultOutStorageNameParameterName
            );
            _outputManager = new IO.Output.OutputManager(defaultStorageName);

            foreach (XElement element in outputManagerElement.Elements())
            {
                IO.Output.IOutputter outputter = _serviceBuilder.CreateOutputter(element);
                _outputManager.Add(outputter);
            }
        }

        /// <inheritdoc />
        /// <exception cref="InvalidOperationException">
        /// XML configuration doesn't contain element for data base manager with specified name.
        /// </exception>
        public void BuildDataBaseManager()
        {
            XElement? dataBaseManagerElement = _documentParser.FindElement(
                _dataBaseManagerParameterName
            );
            if (dataBaseManagerElement is null)
            {
                throw new InvalidOperationException(
                    $"XML document has not value for {_dataBaseManagerParameterName}."
                );
            }

            var databaseOptions = Configuration.ConfigOptions.GetOptions<DataAccessLayer.DatabaseOptions>();
            _databaseManager = new DataAccessLayer.DatabaseManager(
                new DataAccessLayer.Repositories.ResultInfoRepository(databaseOptions),
                new DataAccessLayer.Repositories.RatingRepository(databaseOptions)
            );

            foreach (XElement element in dataBaseManagerElement.Elements())
            {
                DataAccessLayer.Repositories.IDataRepository repository = _serviceBuilder.CreateRepository(
                    element, databaseOptions
                );
                _databaseManager.DataRepositoriesManager.Add(repository);
            }
        }

        /// <inheritdoc />
        public Shell GetResult()
        {
            if (_inputManager is null)
            {
                throw new InvalidOperationException(
                    $"{nameof(IO.Input.InputManager)} was not built."
                );
            }
            if (_crawlersManager is null)
            {
                throw new InvalidOperationException(
                     $"{nameof(Crawlers.CrawlersManager)} was not built."
                );
            }
            if (_appraisersManager is null)
            {
                throw new InvalidOperationException(
                     $"{nameof(Appraisers.AppraisersManager)} was not built."
                );
            }
            if (_outputManager is null)
            {
                throw new InvalidOperationException(
                    $"{nameof(IO.Output.OutputManager)} was not built."
                );
            }
            if (_databaseManager is null)
            {
                throw new InvalidOperationException(
                    $"{nameof(DataAccessLayer.DatabaseManager)} was not built."
                );
            }

            _logger.Info($"Creating {nameof(Shell)} from user-defined XML config.");

            return new Shell(_inputManager, _crawlersManager, _appraisersManager, _outputManager,
                             _databaseManager);
        }

        #endregion
    }
}
