using System;
using System.Xml.Linq;
using ThingAppraiser.Logging;

namespace ThingAppraiser.Core
{
    /// <summary>
    /// Provides template methods to generate XML configuration for <see cref="CShell" /> class. 
    /// XML configuration could be used by <see cref="Building.CShellBuilderFromXDocument" /> to 
    /// initialize <see cref="CShell" /> instances.
    /// </summary>
    /// <remarks>
    /// Structure of XML config must satisfy certain contracts, otherwise different exception could
    /// be thrown during <see cref="CShell" /> building process.
    /// If you add your own message handler, make sure that you provide appropriate builder
    /// which can parse XML document with your attributes and elements.
    /// </remarks>
    public class CXMLConfigCreator
    {
        /// <summary>
        /// Attribute name for root element of config.
        /// </summary>
        public static String RootParameterName { get; } = "ShellConfig";

        /// <summary>
        /// Element name for message handler part.
        /// </summary>
        public static String MessageHandlerParameterName { get; } = "MessageHandler";

        /// <summary>
        /// Attribute name for message handler type.
        /// </summary>
        public static String MessageHandlerTypeParameterName { get; } = "MessageHandlerType";

        /// <summary>
        /// Element name for input manager part.
        /// </summary>
        public static String InputManagerParameterName { get; } = "InputManager";

        /// <summary>
        /// Element name for crawlers manager part.
        /// </summary>
        public static String CrawlersManagerParameterName { get; } = "CrawlersManager";

        /// <summary>
        /// Element name for appraisers manager part.
        /// </summary>
        public static String AppraisersManagerParameterName { get; } = "AppraisersManager";

        /// <summary>
        /// Element name for output manager part.
        /// </summary>
        public static String OutputManagerParameterName { get; } = "OutputManager";

        /// <summary>
        /// Element name for data base manager part.
        /// </summary>
        public static String DataBaseManagerParameterName { get; } = "DataBaseManager";

        /// <summary>
        /// Logger instance for current class.
        /// </summary>
        private static readonly CLoggerAbstraction s_logger =
            CLoggerAbstraction.CreateLoggerInstanceFor<CXMLConfigCreator>();

        /// <summary>
        /// Represents result of the generating process.
        /// </summary>
        private XDocument _result;


        /// <summary>
        /// Initializes class with basic template structure for XML configuration.
        /// </summary>
        public CXMLConfigCreator()
        {
            Reset();
        }

        /// <summary>
        /// Creates default configuration of the service. If you want to customize components, use
        /// other methods to define what you want.
        /// </summary>
        /// <returns>Default XML config of service.</returns>
        public static XDocument CreateDefaultXMLConfig()
        {
            var xmlConfig = new XDocument(
                new XElement(RootParameterName,
                    new XElement(MessageHandlerParameterName,
                        new XAttribute(MessageHandlerTypeParameterName, "ConsoleMessageHandler"),
                        new XAttribute("ConsoleMessageHandlerSetUnicode", "true")
                    ),
                    new XElement(InputManagerParameterName,
                        new XAttribute("DefaultInFilename", "thing_names.txt"),
                        //new XElement("LocalFile",
                        //    new XAttribute("FileReaderLocalFile", "Simple")
                        //)
                        new XElement("GoogleDrive",
                            new XAttribute("FileReaderGoogleDrive", "Simple")
                        )
                    ),
                    new XElement(CrawlersManagerParameterName,
                        new XAttribute("CrawlersOutputFlag", "false"),
                        new XElement("CrawlerTMDB",
                            new XAttribute("APIKeyTMDB", "f7440a70737103fea00fb6e8352a3533"),
                            new XAttribute("SearchUrlTMDB",
                                           "https://api.themoviedb.org/3/search/movie"),
                            new XAttribute("ConfigurationUrlTMDB",
                                           "https://api.themoviedb.org/3/configuration"),
                            new XAttribute("RequestsPerTimeTMDB", "30"),
                            new XAttribute("GoodStatusCodeTMDB", "200"),
                            new XAttribute("LimitAttemptsTMDB", "10"),
                            new XAttribute("MillisecondsTimeoutTMDB", "1000")
                        )
                    ),
                    new XElement(AppraisersManagerParameterName,
                        new XAttribute("AppraisersOutputFlag", "false"),
                        //new XElement("AppraiserTMDB"),
                        new XElement("FuzzyAppraiserTMDB")
                    ),
                    new XElement(OutputManagerParameterName,
                        new XAttribute("DefaultOutFilename", "appraised_things.csv"),
                        new XElement("LocalFile"),
                        new XElement("GoogleDrive")
                    ),
                    new XElement(DataBaseManagerParameterName,
                        new XAttribute("ConnectionString", @"Data Source=(localdb)\MSSQLLocalDB;
Initial Catalog=thing_appraiser;Integrated Security=True;Connect Timeout=30;Encrypt=False;
TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"),
                        new XElement("MovieTMDBRepository")
                    )
                )
            );

            return xmlConfig;
        }

        /// <summary>
        /// Resets all changes to basic template of XML configuration.
        /// </summary>
        public void Reset()
        {
            _result = new XDocument(
                new XElement(RootParameterName,
                    new XElement(MessageHandlerParameterName),
                    new XElement(InputManagerParameterName),
                    new XElement(CrawlersManagerParameterName),
                    new XElement(AppraisersManagerParameterName),
                    new XElement(OutputManagerParameterName),
                    new XElement(DataBaseManagerParameterName)
                )
            );
        }

        /// <summary>
        /// Adds type of message handler. Method knows where this attribute value should place, you
        /// should only specify name of the message handler.
        /// </summary>
        /// <param name="messageHandlerTypeAttributeValue">
        /// Value of the message handler attribute.
        /// </param>
        /// <remarks>
        /// If you add your own message handler, make sure that you provide appropriate builder
        /// which can parse XML document with your attributes and elements.
        /// </remarks>
        /// <exception cref="ArgumentException">
        /// <param name="messageHandlerTypeAttributeValue">messageHandlerTypeAttribute</param> is
        /// null or presents empty string.
        /// </exception>
        public void AddMessageHandlerType(String messageHandlerTypeAttributeValue)
        {
            messageHandlerTypeAttributeValue.ThrowIfNullOrEmpty(
                nameof(messageHandlerTypeAttributeValue)
            );

            AddAttribute(
                MessageHandlerParameterName,
                new XAttribute(MessageHandlerTypeParameterName, messageHandlerTypeAttributeValue)
            );
        }

        /// <summary>
        /// Adds parameter for message handler. Method knows where this attribute should place.
        /// </summary>
        /// <param name="messageHandlerAttribute">
        /// XML attribute which represents parameter for instance of 
        /// <see cref="Communication.IMessageHandler"/> which sets in
        /// <see cref="AddMessageHandlerType"/>.
        /// </param>
        public void AddMessageHandlerAttribute(XAttribute messageHandlerAttribute)
        {
            AddAttribute(MessageHandlerParameterName, messageHandlerAttribute);
        }

        /// <summary>
        /// Adds parameter for input manager. Method knows where this attribute should place.
        /// </summary>
        /// <param name="inputManagerAttribute">
        /// XML attribute which represents parameter for <see cref="IO.Input.CInputManager" />.
        /// </param>
        public void AddInputManagerAttribute(XAttribute inputManagerAttribute)
        {
            AddAttribute(InputManagerParameterName, inputManagerAttribute);
        }

        /// <summary>
        /// Adds new element for input manager part of XML configuration.
        /// </summary>
        /// <param name="inputterElement">XML element to add.</param>
        public void AddInputterElement(XElement inputterElement)
        {
            AddElement(InputManagerParameterName, inputterElement);
        }

        /// <summary>
        /// Adds parameter for crawlers manager. Method knows where this attribute should place.
        /// </summary>
        /// <param name="crawlersManagerAttribute">
        /// XML attribute which represents parameter for <see cref="Crawlers.CCrawlersManager" />.
        /// </param>
        public void AddCrawlersManagerAttribute(XAttribute crawlersManagerAttribute)
        {
            AddAttribute(CrawlersManagerParameterName, crawlersManagerAttribute);
        }

        /// <summary>
        /// Adds new element for crawlers manager part of XML configuration.
        /// </summary>
        /// <param name="crawlerElement">XML element to add.</param>
        public void AddCrawlerElement(XElement crawlerElement)
        {
            AddElement(CrawlersManagerParameterName, crawlerElement);
        }

        /// <summary>
        /// Adds parameter for appraisers manager. Method knows where this attribute should place.
        /// </summary>
        /// <param name="appraisersManagerAttribute">
        /// XML attribute which represents parameter for
        /// <see cref="Appraisers.CAppraisersManager" />.
        /// </param>
        public void AddAppraisersManagerAttribute(XAttribute appraisersManagerAttribute)
        {
            AddAttribute(AppraisersManagerParameterName, appraisersManagerAttribute);
        }

        /// <summary>
        /// Adds new element for appraisers manager part of XML configuration.
        /// </summary>
        /// <param name="appraiserElement">XML element to add.</param>
        public void AddAppraiserElement(XElement appraiserElement)
        {
            AddElement(AppraisersManagerParameterName, appraiserElement);
        }

        /// <summary>
        /// Adds parameter for output manager. Method knows where this attribute should place.
        /// </summary>
        /// <param name="outputManagerAttribute">
        /// XML attribute which represents parameter for <see cref="IO.Output.COutputManager" />.
        /// </param>
        public void AddOutputManagerAttribute(XAttribute outputManagerAttribute)
        {
            AddAttribute(OutputManagerParameterName, outputManagerAttribute);
        }

        /// <summary>
        /// Adds new element for output manager part of XML configuration.
        /// </summary>
        /// <param name="outputterElement">XML element to add.</param>
        public void AddOutputterElement(XElement outputterElement)
        {
            AddElement(OutputManagerParameterName, outputterElement);
        }

        /// <summary>
        /// Adds parameter for data base manager. Method knows where this attribute should place.
        /// </summary>
        /// <param name="dataBaseManagerAttribute">
        /// XML attribute which represents parameter for <see cref="DAL.CDataBaseManager" />.
        /// </param>
        public void AddDataBaseManagerAttribute(XAttribute dataBaseManagerAttribute)
        {
            AddAttribute(DataBaseManagerParameterName, dataBaseManagerAttribute);
        }

        /// <summary>
        /// Adds new element for data base manager part of XML configuration.
        /// </summary>
        /// <param name="repositoryElement">XML element to add.</param>
        public void AddRepositoryElement(XElement repositoryElement)
        {
            AddElement(DataBaseManagerParameterName, repositoryElement);
        }

        /// <summary>
        /// Returns current version of XML configuration.
        /// </summary>
        /// <returns>Created XML configuration.</returns>
        public XDocument GetResult()
        {
            return new XDocument(_result);
        }

        /// <summary>
        /// Finds element by name and adds new XML attribute to it.
        /// </summary>
        /// <param name="elementName">Name of the element to find.</param>
        /// <param name="newAttribute">XML attribute to add.</param>
        private void AddAttribute(XName elementName, XAttribute newAttribute)
        {
            Add(elementName, newAttribute);
        }
        
        /// <summary>
        /// Finds element by name and adds new XML element to it.
        /// </summary>
        /// <param name="elementName">Name of the element to find.</param>
        /// <param name="newElement">XML element to add.</param>
        private void AddElement(XName elementName, XElement newElement)
        {
            Add(elementName, newElement);
        }

        /// <summary>
        /// Finds element by name and adds new content to it.
        /// </summary>
        /// <param name="elementName">Name of the element to find.</param>
        /// <param name="content">Content to add.</param>
        /// <exception cref="InvalidOperationException">
        /// Document doesn't have Root element because of uninitialized variable.
        /// -or-Element with specified name doesn't exist in the document.
        /// </exception>
        private void Add(XName elementName, Object content)
        {
            elementName.ThrowIfNull(nameof(elementName));
            content.ThrowIfNull(nameof(content));

            if (_result.Root is null)
            {
                var ex = new InvalidOperationException(
                    "XML document doesn't have Root element yet."
                );
                s_logger.Error(ex, "Tried to add content in document without Root element.");
                throw ex;
            }

            var foundElement = _result.Root.Element(elementName);
            if (foundElement is null)
            {
                var ex = new InvalidOperationException(
                    $"XML document doesn't have specified element: {elementName}"
                );
                s_logger.Error(ex, "Tried to add content in document without required element.");
                throw ex;
            }

            foundElement.Add(content);
        }
    }
}
