using System;
using System.Xml.Linq;
using ThingAppraiser.Logging;

namespace ThingAppraiser.Core.Building
{
    /// <summary>
    /// Builder class which provides the way of constructing <see cref="CShell" /> instances from
    /// <see cref="XDocument" /> config.
    /// </summary>
    /// <remarks>
    /// Structure of XML config must satisfy certain contracts, otherwise different exception could
    /// be thrown.
    /// </remarks>
    public sealed class CShellBuilderFromXDocument : IShellBuilder
    {
        /// <summary>
        /// Logger instance for current class.
        /// </summary>
        private static readonly CLoggerAbstraction s_logger =
            CLoggerAbstraction.CreateLoggerInstanceFor<CShellBuilderFromXDocument>();

        /// <summary>
        /// Element name for message handler part.
        /// </summary>
        private readonly String _messageHandlerParameterName =
            CXMLConfigCreator.MessageHandlerParameterName;

        /// <summary>
        /// Element name for input manager part.
        /// </summary>
        private readonly String _inputManagerParameterName =
            CXMLConfigCreator.InputManagerParameterName;

        /// <summary>
        /// Attribute name for default input filename of input manager.
        /// </summary>
        private readonly String _defaultInFilenameParameterName = "DefaultInFilename";

        /// <summary>
        /// Element name for crawlers manager part.
        /// </summary>
        private readonly String _crawlersManagerParameterName =
            CXMLConfigCreator.CrawlersManagerParameterName;

        /// <summary>
        /// Attribute name for crawlers output flag of crawlers manager.
        /// </summary>
        private readonly String _crawlersOutputParameterName = "CrawlersOutputFlag";

        /// <summary>
        /// Element name for appraisers manager part.
        /// </summary>
        private readonly String _appraisersManagerParameterName =
            CXMLConfigCreator.AppraisersManagerParameterName;

        /// <summary>
        /// Attribute name for appraisers output flag of appraisers manager.
        /// </summary>
        private readonly String _appraisersOutputParameterName = "AppraisersOutputFlag";

        /// <summary>
        /// Element name for output manager part.
        /// </summary>
        private readonly String _outputManagerParameterName =
            CXMLConfigCreator.OutputManagerParameterName;

        /// <summary>
        /// Attribute name for default output filename of output manager.
        /// </summary>
        private readonly String _defaultOutFilenameParameterName = "DefaultOutFilename";

        /// <summary>
        /// Helper class which contains several methods to parse XML configuration.
        /// </summary>
        private readonly CXDocumentParser _documentParser;

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
        /// Initializes builder instance and associates <see cref="CXDocumentParser" /> which
        /// provides deferred parsing of XML configuration.
        /// </summary>
        /// <param name="configuration">XML configuration of <see cref="CShell" /> class.</param>
        public CShellBuilderFromXDocument(XDocument configuration)
        {
            _documentParser = new CXDocumentParser(configuration);
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
        }

        /// <inheritdoc />
        public void BuildMessageHandler()
        {
            XElement messageHandlerElement = _documentParser.FindElement(
                _messageHandlerParameterName
            );

            Communication.SGlobalMessageHandler.MessageHandler =
                SServiceBuilder.CreateMessageHandlerWithXMLParameters(messageHandlerElement);
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
                s_logger.Error(ex, "Cannot build InputManager.");
                throw ex;
            }

            String defaultFilename = CXDocumentParser.GetAttributeValue(
                inputManagerElement, _defaultInFilenameParameterName
            );

            if (String.IsNullOrEmpty(defaultFilename))
            {
                _inputManager = new IO.Input.CInputManager();
            }
            else
            {
                _inputManager = new IO.Input.CInputManager(defaultFilename);
            }

            foreach (var element in inputManagerElement.Elements())
            {
                IO.Input.IInputter inputter = SServiceBuilder.CreateInputterWithXMLParameters(
                    element
                );
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
                s_logger.Error(ex, "Cannot build CrawlersManager.");
                throw ex;
            }

            var crawlersOutput = CXDocumentParser.GetAttributeValue<Boolean>(
                crawlerManagerElement, _crawlersOutputParameterName
            );
            _crawlersManager = new Crawlers.CCrawlersManager(crawlersOutput);

            foreach (var element in crawlerManagerElement.Elements())
            {
                Crawlers.CCrawler crawler = SServiceBuilder.CreateCrawlerWithXMLParameters(element);
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
                s_logger.Error(ex, "Cannot build AppraisersManager.");
                throw ex;
            }

            var appraisersOutput = CXDocumentParser.GetAttributeValue<Boolean>(
                appraiserManagerElement, _appraisersOutputParameterName
            );
            _appraisersManager = new Appraisers.CAppraisersManager(appraisersOutput);

            foreach (var element in appraiserManagerElement.Elements())
            {
                Appraisers.CAppraiser crawler = SServiceBuilder.CreateAppraiserWithXMLParameters(
                    element
                );
                _appraisersManager.Add(crawler);
            }
        }

        /// <inheritdoc />
        /// <exception cref="InvalidOperationException">
        /// XML configuration doesn't contain element for output manager with specified name.
        /// </exception>
        public void BuildOutputManager()
        {
            XElement inputManagerElement = _documentParser.FindElement(_outputManagerParameterName);
            if (inputManagerElement is null)
            {
                var ex = new InvalidOperationException(
                    $"XML document hasn't value for {_outputManagerParameterName}."
                );
                s_logger.Error(ex, "Cannot build OutputManager.");
                throw ex;
            }

            String defaultFilename = CXDocumentParser.GetAttributeValue(
                inputManagerElement, _defaultOutFilenameParameterName
            );

            if (String.IsNullOrEmpty(defaultFilename))
            {
                _outputManager = new IO.Output.COutputManager();
            }
            else
            {
                _outputManager = new IO.Output.COutputManager(defaultFilename);
            }

            foreach (var element in inputManagerElement.Elements())
            {
                IO.Output.IOutputter outputter = SServiceBuilder.CreateOutputterWithXMLParameters(
                    element
                );
                _outputManager.Add(outputter);
            }
        }

        /// <inheritdoc />
        public CShell GetResult()
        {
            s_logger.Info("Created Shell from user-defined XML config.");
            return new CShell(_inputManager, _crawlersManager, _appraisersManager, _outputManager);
        }

        #endregion
    }
}
