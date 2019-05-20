using System;
using System.Xml.Linq;
using ThingAppraiser.Logging;

namespace ThingAppraiser.Core.Building
{
    /// <summary>
    /// Builder class which provides the way of constructing <see cref="ShellRx" /> instances 
    /// from <see cref="XDocument" /> config.
    /// </summary>
    /// <remarks>
    /// Structure of XML config must satisfy certain contracts, otherwise different exception could
    /// be thrown.
    /// </remarks>
    public sealed class ShellRxBuilderFromXDocument : ShellBuilderBase, IShellRxBuilder
    {
        /// <summary>
        /// Logger instance for current class.
        /// </summary>
        private static readonly LoggerAbstraction _logger =
            LoggerAbstraction.CreateLoggerInstanceFor<ShellRxBuilderFromXDocument>();

        /// <summary>
        /// Provides methods to create instances of service classes.
        /// </summary>
        private readonly ServiceRxBuilderForXmlConfig _serviceBuilder =
            new ServiceRxBuilderForXmlConfig();

        /// <summary>
        /// Helper class which contains several methods to parse XML configuration.
        /// </summary>
        private readonly XDocumentParser _documentParser;

        /// <summary>
        /// Default value of bounded capacity of service.
        /// </summary>
        private readonly int _defaultBoundedCapacity = 10;

        /// <summary>
        /// Variables which saves input manager instance during building process.
        /// </summary>
        private IO.Input.InputManagerRx _inputManager;

        /// <summary>
        /// Variables which saves crawlers manager instance during building process.
        /// </summary>
        private Crawlers.CrawlersManagerRx _crawlersManager;

        /// <summary>
        /// Variables which saves appraisers manager instance during building process.
        /// </summary>
        private Appraisers.AppraisersManagerRx _appraisersManager;

        /// <summary>
        /// Variables which saves output manager instance during building process.
        /// </summary>
        private IO.Output.OutputManagerRx _outputManager;


        /// <summary>
        /// Initializes builder instance and associates <see cref="XDocumentParser" /> which
        /// provides deferred parsing of XML configuration.
        /// </summary>
        /// <param name="configuration">XML configuration of <see cref="ShellRx" /> class.</param>
        public ShellRxBuilderFromXDocument(XDocument configuration)
        {
            _documentParser = new XDocumentParser(
                new XDocument(configuration.Root.Element(_rootElementName))
            );
        }

        #region IShellRxBuilder Implementation

        /// <inheritdoc />
        public void Reset()
        {
            _inputManager = null;
            _crawlersManager = null;
            _appraisersManager = null;
            _outputManager = null;
        }

        /// <inheritdoc />
        public void BuildMessageHandler()
        {
            XElement messageHandlerElement = _documentParser.FindElement(
                _messageHandlerParameterName
            );

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
            XElement inputManagerElement = _documentParser.FindElement(_inputManagerParameterName);
            if (inputManagerElement is null)
            {
                var ex = new InvalidOperationException(
                    $"XML document hasn't value for {_inputManagerParameterName}."
                );
                _logger.Error(ex, "Cannot build InputManager.");
                throw ex;
            }

            string defaultStorageName = XDocumentParser.GetAttributeValue(
                inputManagerElement, _defaultInStorageNameParameterName
            );
            _inputManager = new IO.Input.InputManagerRx(defaultStorageName);

            foreach (var element in inputManagerElement.Elements())
            {
                IO.Input.IInputterRx inputter = _serviceBuilder.CreateInputter(element);
                _inputManager.Add(inputter);
            }
        }

        /// <inheritdoc />
        /// <exception cref="InvalidOperationException">
        /// XML configuration doesn't contain element for crawlers manager with specified name.
        /// </exception>
        public void BuildCrawlersManager()
        {
            XElement crawlerManagerElement = _documentParser.FindElement(
                _crawlersManagerParameterName
            );
            if (crawlerManagerElement is null)
            {
                var ex = new InvalidOperationException(
                    $"XML document hasn't value for {_crawlersManagerParameterName}."
                );
                _logger.Error(ex, "Cannot build CrawlersManager.");
                throw ex;
            }

            var crawlersOutput = XDocumentParser.GetAttributeValue<bool>(
                crawlerManagerElement, _crawlersOutputParameterName
            );
            _crawlersManager = new Crawlers.CrawlersManagerRx(crawlersOutput);

            foreach (var element in crawlerManagerElement.Elements())
            {
                Crawlers.CrawlerRx crawler = _serviceBuilder.CreateCrawler(element);
                _crawlersManager.Add(crawler);
            }
        }

        /// <inheritdoc />
        /// <exception cref="InvalidOperationException">
        /// XML configuration doesn't contain element for appraisers manager with specified name.
        /// </exception>
        public void BuildAppraisersManager()
        {
            XElement appraiserManagerElement = _documentParser.FindElement(
                _appraisersManagerParameterName
            );
            if (appraiserManagerElement is null)
            {
                var ex = new InvalidOperationException(
                    $"XML document hasn't value for {_appraisersManagerParameterName}."
                );
                _logger.Error(ex, "Cannot build AppraisersManager.");
                throw ex;
            }

            var appraisersOutput = XDocumentParser.GetAttributeValue<bool>(
                appraiserManagerElement, _appraisersOutputParameterName
            );
            _appraisersManager = new Appraisers.AppraisersManagerRx(appraisersOutput);

            foreach (var element in appraiserManagerElement.Elements())
            {
                Appraisers.AppraiserRx crawler = _serviceBuilder.CreateAppraiser(element);
                _appraisersManager.Add(crawler);
            }
        }

        /// <inheritdoc />
        /// <exception cref="InvalidOperationException">
        /// XML configuration doesn't contain element for output manager with specified name.
        /// </exception>
        public void BuildOutputManager()
        {
            XElement outputManagerElement = _documentParser.FindElement(
                _outputManagerParameterName
            );
            if (outputManagerElement is null)
            {
                var ex = new InvalidOperationException(
                    $"XML document hasn't value for {_outputManagerParameterName}."
                );
                _logger.Error(ex, "Cannot build OutputManager.");
                throw ex;
            }

            string defaultStorageName = XDocumentParser.GetAttributeValue(
                outputManagerElement, _defaultOutStorageNameParameterName
            );
            _outputManager = new IO.Output.OutputManagerRx(defaultStorageName);

            foreach (var element in outputManagerElement.Elements())
            {
                IO.Output.IOutputterRx outputter = _serviceBuilder.CreateOutputter(element);
                _outputManager.Add(outputter);
            }
        }

        /// <inheritdoc />
        public ShellRx GetResult()
        {
            _logger.Info("Created ShellRx from user-defined XML config.");
            return new ShellRx(_inputManager, _crawlersManager, _appraisersManager,
                               _outputManager, _defaultBoundedCapacity);
        }

        #endregion
    }
}
