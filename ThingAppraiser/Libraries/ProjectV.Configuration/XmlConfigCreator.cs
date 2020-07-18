using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Acolyte.Assertions;
using Acolyte.Xml;
using ProjectV.Logging;
using ProjectV.Models.Configuration;
using ProjectV.Models.WebService;

namespace ProjectV.Configuration
{
    /// <summary>
    /// Provides template methods to generate XML configuration for shell instance.
    /// XML configuration could be used by builder to initialize pipeline and get instance to work.
    /// </summary>
    /// <remarks>
    /// Structure of XML config must satisfy certain contracts, otherwise different exception could
    /// be thrown during shell building process.
    /// If you add your own message handler, make sure that you provide appropriate builder
    /// which can parse XML document with your attributes and elements.
    /// </remarks>
    public sealed class XmlConfigCreator
    {
        /// <summary>
        /// Logger instance for current class.
        /// </summary>
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor<XmlConfigCreator>();

        /// <summary>
        /// Inner array to hold collection of message handler parameters.
        /// </summary>
        private readonly List<XElement> _messageHandlerParameters = new List<XElement>();

        /// <summary>
        /// Inner array to hold collection of input manager parameters.
        /// </summary>
        private readonly List<XElement> _inputManagerParameters = new List<XElement>();

        /// <summary>
        /// Inner array to hold collection of inputters.
        /// </summary>
        private readonly List<XElement> _inputters = new List<XElement>();

        /// <summary>
        /// Inner array to hold collection of crawlers manager parameters.
        /// </summary>
        private readonly List<XElement> _crawlersManagerParameters = new List<XElement>();

        /// <summary>
        /// Inner array to hold collection of crawlers.
        /// </summary>
        private readonly List<XElement> _crawlers = new List<XElement>();

        /// <summary>
        /// Inner array to hold collection of appraisers manager parameters.
        /// </summary>
        private readonly List<XElement> _appraisersManagerParameters = new List<XElement>();

        /// <summary>
        /// Inner array to hold collection of appraisers.
        /// </summary>
        private readonly List<XElement> _appraisers = new List<XElement>();

        /// <summary>
        /// Inner array to hold collection of output manager parameters.
        /// </summary>
        private readonly List<XElement> _outputManagerParameters = new List<XElement>();

        /// <summary>
        /// Inner array to hold collection of outputters.
        /// </summary>
        private readonly List<XElement> _outputters = new List<XElement>();

        /// <summary>
        /// Inner array to hold collection of data base parameters.
        /// </summary>
        private readonly List<XElement> _dataBaseManagerParameters = new List<XElement>();

        /// <summary>
        /// Inner array to hold collection of data base repositories.
        /// </summary>
        private readonly List<XElement> _repositories = new List<XElement>();

        /// <summary>
        /// Represents result of the generating process.
        /// </summary>
        private ConfigurationXml _result = default!; // Initializes in method call in ctor.


        /// <summary>
        /// Initializes creator with new empty list.
        /// </summary>
        public XmlConfigCreator()
        {
            Reset();
        }

        /// <summary>
        /// Creates default configuration of the service. If you want to customize components, use
        /// other methods to define what you want.
        /// </summary>
        /// <returns>Default XML config of service as serialized set of classes.</returns>
        public static ConfigurationXml CreateDefaultXmlConfig()
        {
            var xmlConfig = new ConfigurationXml
            {
                ShellConfig = new ShellConfig
                {
                    MessageHandler = new MessageHandlerConfig
                    {
                        MessageHandlerType = "ConsoleMessageHandler",
                        MessageHandlerParameters = new[]
                        {
                            ConfigModule.GetConfigForMessageHandlerParameter(
                                ConfigNames.MessageHandlerParameters
                                    .ConsoleMessageHandlerSetUnicodeName
                            )
                        }
                    },
                    InputManager = new InputManagerConfig
                    {
                        DefaultInStorageName = "thing_names.csv",
                        Inputters = new[]
                        {
                            ConfigModule.GetConfigForInputter(
                                ConfigNames.Inputters.LocalFileReaderSimpleName
                            ),
                        }
                    },
                    CrawlersManager = new CrawlersManagerConfig
                    {
                        CrawlersOutputFlag = false,
                        Crawlers = new[]
                        {
                            ConfigModule.GetConfigForCrawler(ConfigNames.Crawlers.TmdbCrawlerName),
                            ConfigModule.GetConfigForCrawler(ConfigNames.Crawlers.OmdbCrawlerName)
                        }
                    },
                    AppraisersManager = new AppraisersManagerConfig
                    {
                        AppraisersOutputFlag = false,
                        Appraisers = new[]
                        {
                            ConfigModule.GetConfigForAppraiser(
                                ConfigNames.Appraisers.TmdbAppraiserCommonName
                            ),
                            ConfigModule.GetConfigForAppraiser(
                                ConfigNames.Appraisers.OmdbAppraiserCommonName
                            )
                        }
                    },
                    OutputManager = new OutputManagerConfig
                    {
                        DefaultOutStorageName = "appraised_thing.csv",
                        Outputters = new[]
                        {
                            ConfigModule.GetConfigForOutputter(
                                ConfigNames.Outputters.LocalFileWriterName
                            ),
                        }
                    },
                    DataBaseManager = new DataBaseManagerConfig
                    {
                        Repositories = new[]
                        {
                            ConfigModule.GetConfigForRepository(
                                ConfigNames.Repositories.TmdbMovieRepositoryName
                            )
                        }
                    }
                },
                ServiceType = ServiceType.TplDataflow
            };

            return xmlConfig;
        }

        /// <summary>
        /// Creates default configuration of the service. If you want to customize components, use
        /// other methods to define what you want.
        /// </summary>
        /// <returns>Default XML config of service as serialized XML value.</returns>
        public static XDocument CreateDefaultXmlConfigAsXDocument()
        {
            var xmlConfig = CreateDefaultXmlConfig();
            return TransformConfigToXDocument(xmlConfig);
        }

        /// <summary>
        /// Transforms config to <see cref="XDocument" /> variable.
        /// </summary>
        /// <returns>XML config of service as instance of <see cref="XDocument" />.</returns>
        /// <remarks>
        /// This method uses serialization to perform transformation.
        /// </remarks>
        public static XDocument TransformConfigToXDocument(ConfigurationXml xmlConfig)
        {
            var stringXml = XmlHelper.SerializeToStringXml(xmlConfig);
            return XDocument.Parse(stringXml);
        }

        /// <summary>
        /// Creates XML configuration according to the specified configuration data.
        /// </summary>
        /// <param name="configRequirements">Data which describes necessary configuration.</param>
        /// <returns>XML config of service as serialized set of classes.</returns>
        public static ConfigurationXml CreateXmlConfigBasedOnRequirements(
            ConfigRequirements configRequirements)
        {
            configRequirements.ThrowIfNull(nameof(configRequirements));

            var xmlConfigCreator = new XmlConfigCreator();

            xmlConfigCreator.SetMessageHandlerType("ConsoleMessageHandler");
            xmlConfigCreator.AddMessageHandlerParameter(
                new XElement("ConsoleMessageHandlerSetUnicode", "false")
            );

            xmlConfigCreator.SetDefaultInStorageName("thing_names.csv");

            xmlConfigCreator.SetCrawlersOutputFlag(false);

            xmlConfigCreator.SetAppraisersOutputFlag(false);

            xmlConfigCreator.SetDefaultOutStorageName("appraised_things.csv");

            xmlConfigCreator.AddRepository(
                ConfigModule.GetConfigForRepository(
                    ConfigNames.Repositories.BasicInfoRepositoryName
                )
            );

            foreach (var inputItem in configRequirements.Input)
            {
                ConfigContract.CheckAvailability(inputItem, ConfigContract.AvailableInput);

                xmlConfigCreator.AddInputter(
                    ConfigModule.GetConfigForInputter(inputItem)
                );
            }

            foreach (var serviceItem in configRequirements.Services)
            {
                ConfigContract.CheckAvailability(serviceItem, ConfigContract.AvailableServices);

                xmlConfigCreator.AddCrawler(
                    ConfigModule.GetConfigForCrawler(serviceItem)
                );
                xmlConfigCreator.AddRepository(
                    ConfigModule.GetConfigForRepository(serviceItem)
                );
            }

            foreach (var appraisalItem in configRequirements.Appraisals)
            {
                ConfigContract.CheckAvailability(appraisalItem, ConfigContract.AvailableAppraisals);

                xmlConfigCreator.AddAppraiser(
                    ConfigModule.GetConfigForAppraiser(appraisalItem)
                );
            }

            foreach (var outputItem in configRequirements.Output)
            {
                ConfigContract.CheckAvailability(outputItem, ConfigContract.AvailableOutput);

                xmlConfigCreator.AddOutputter(
                    ConfigModule.GetConfigForOutputter(outputItem)
                );
            }

            xmlConfigCreator.SetServiceType(ServiceType.TplDataflow);

            return xmlConfigCreator.GetResult();
        }

        /// <summary>
        /// Resets all changes to basic template of XML configuration.
        /// </summary>
        public void Reset()
        {
            _result = new ConfigurationXml
            {
                ShellConfig = new ShellConfig
                {
                    MessageHandler = new MessageHandlerConfig(),
                    InputManager = new InputManagerConfig(),
                    CrawlersManager = new CrawlersManagerConfig(),
                    AppraisersManager = new AppraisersManagerConfig(),
                    OutputManager = new OutputManagerConfig(),
                    DataBaseManager = new DataBaseManagerConfig()
                }
            };

            _messageHandlerParameters.Clear();

            _inputManagerParameters.Clear();
            _inputters.Clear();

            _crawlersManagerParameters.Clear();
            _crawlers.Clear();

            _appraisersManagerParameters.Clear();
            _appraisers.Clear();

            _outputManagerParameters.Clear();
            _outputters.Clear();

            _dataBaseManagerParameters.Clear();
            _repositories.Clear();
        }

        /// <summary>
        /// Sets type of message handler. Method knows where this attribute value should place,
        /// you should only specify name of the message handler.
        /// </summary>
        /// <param name="messageHandlerType">Message handler type.</param>
        /// <remarks>
        /// If you add your own message handler, make sure that you provide appropriate builder
        /// which can parse XML document with your attributes and elements.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="messageHandlerType" /> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="messageHandlerType" /> presents empty string.
        /// </exception>
        public void SetMessageHandlerType(string messageHandlerType)
        {
            messageHandlerType.ThrowIfNullOrEmpty(nameof(messageHandlerType));

            _result.ShellConfig.MessageHandler.MessageHandlerType = messageHandlerType;
        }

        /// <summary>
        /// Adds parameter for message handler. Method knows where this element should place.
        /// </summary>
        /// <param name="messageHandlerParameter">
        /// XML element which represents parameter for instance of 
        /// <see cref="Communication.IMessageHandler" /> which sets in
        /// <see cref="SetMessageHandlerType" />.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="messageHandlerParameter" /> is <c>null</c>.
        /// </exception>
        public void AddMessageHandlerParameter(XElement messageHandlerParameter)
        {
            messageHandlerParameter.ThrowIfNull(nameof(messageHandlerParameter));

            _messageHandlerParameters.Add(messageHandlerParameter);
        }

        /// <summary>
        /// Sets default storage name for input manager. Method knows where this attribute value 
        /// should place, you should only specify value of the storage name.
        /// </summary>
        /// <param name="defaultInStorageName">Default storage name flag for input manager.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="defaultInStorageName" /> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="defaultInStorageName" /> presents empty string.
        /// </exception>
        public void SetDefaultInStorageName(string defaultInStorageName)
        {
            defaultInStorageName.ThrowIfNullOrEmpty(nameof(defaultInStorageName));

            _result.ShellConfig.InputManager.DefaultInStorageName = defaultInStorageName;
        }

        /// <summary>
        /// Adds parameter for input manager. Method knows where this element should place.
        /// </summary>
        /// <param name="inputManagerParameter">
        /// XML element which represents parameter for <see cref="IO.Input.InputManager" />.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="inputManagerParameter" /> is <c>null</c>.
        /// </exception>
        public void AddInputManagerParameter(XElement inputManagerParameter)
        {
            inputManagerParameter.ThrowIfNull(nameof(inputManagerParameter));

            _inputManagerParameters.Add(inputManagerParameter);
        }

        /// <summary>
        /// Adds new element for input manager part of XML configuration.
        /// </summary>
        /// <param name="inputter">XML element to add.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="inputter" /> is <c>null</c>.
        /// </exception>
        public void AddInputter(XElement inputter)
        {
            inputter.ThrowIfNull(nameof(inputter));

            _inputters.Add(inputter);
        }

        /// <summary>
        /// Sets verbose flag for crawlers. Method knows where this attribute value should place, 
        /// you should only specify value of the flag.
        /// </summary>
        /// <param name="crawlersOutputFlag">Verbose flag for crawlers manager.</param>
        public void SetCrawlersOutputFlag(bool crawlersOutputFlag)
        {
            _result.ShellConfig.CrawlersManager.CrawlersOutputFlag = crawlersOutputFlag;
        }

        /// <summary>
        /// Adds parameter for crawlers manager. Method knows where this element should place.
        /// </summary>
        /// <param name="crawlersManagerParameter">
        /// XML element which represents parameter for <see cref="Crawlers.CrawlersManager" />.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="crawlersManagerParameter" /> is <c>null</c>.
        /// </exception>
        public void AddCrawlersManagerParameter(XElement crawlersManagerParameter)
        {
            crawlersManagerParameter.ThrowIfNull(nameof(crawlersManagerParameter));

            _crawlersManagerParameters.Add(crawlersManagerParameter);
        }

        /// <summary>
        /// Adds new element for crawlers manager part of XML configuration.
        /// </summary>
        /// <param name="crawler">XML element to add.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="crawler" /> is <c>null</c>.
        /// </exception>
        public void AddCrawler(XElement crawler)
        {
            crawler.ThrowIfNull(nameof(crawler));

            _crawlers.Add(crawler);
        }

        /// <summary>
        /// Sets verbose flag for appraisers. Method knows where this attribute value should place, 
        /// you should only specify value of the flag.
        /// </summary>
        /// <param name="appraisersOutputFlag">Verbose flag for appraisers manager.</param>
        public void SetAppraisersOutputFlag(bool appraisersOutputFlag)
        {
            _result.ShellConfig.AppraisersManager.AppraisersOutputFlag = appraisersOutputFlag;
        }

        /// <summary>
        /// Adds parameter for appraisers manager. Method knows where this element should place.
        /// </summary>
        /// <param name="appraisersManagerParameter">
        /// XML element which represents parameter for
        /// <see cref="Appraisers.AppraisersManager" />.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="appraisersManagerParameter" /> is <c>null</c>.
        /// </exception>
        public void AddAppraisersManagerParameter(XElement appraisersManagerParameter)
        {
            appraisersManagerParameter.ThrowIfNull(nameof(appraisersManagerParameter));

            _appraisersManagerParameters.Add(appraisersManagerParameter);
        }

        /// <summary>
        /// Adds new element for appraisers manager part of XML configuration.
        /// </summary>
        /// <param name="appraiser">XML element to add.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="appraiser" /> is <c>null</c>.
        /// </exception>
        public void AddAppraiser(XElement appraiser)
        {
            appraiser.ThrowIfNull(nameof(appraiser));

            _appraisers.Add(appraiser);
        }

        /// <summary>
        /// Sets default storage name for output manager. Method knows where this attribute value 
        /// should place, you should only specify value of the storage name.
        /// </summary>
        /// <param name="defaultOutStorageName">
        /// Default storage name flag for output manager.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="defaultOutStorageName" /> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="defaultOutStorageName" /> presents empty string.
        /// </exception>
        public void SetDefaultOutStorageName(string defaultOutStorageName)
        {
            defaultOutStorageName.ThrowIfNullOrEmpty(nameof(defaultOutStorageName));

            _result.ShellConfig.OutputManager.DefaultOutStorageName = defaultOutStorageName;
        }

        /// <summary>
        /// Adds parameter for output manager. Method knows where this element should place.
        /// </summary>
        /// <param name="outputManagerParameter">
        /// XML element which represents parameter for <see cref="IO.Output.OutputManager" />.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="outputManagerParameter" /> is <c>null</c>.
        /// </exception>
        public void AddOutputManagerParameter(XElement outputManagerParameter)
        {
            outputManagerParameter.ThrowIfNull(nameof(outputManagerParameter));

            _outputManagerParameters.Add(outputManagerParameter);
        }

        /// <summary>
        /// Adds new element for output manager part of XML configuration.
        /// </summary>
        /// <param name="outputter">XML element to add.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="outputter" /> is <c>null</c>.
        /// </exception>
        public void AddOutputter(XElement outputter)
        {
            outputter.ThrowIfNull(nameof(outputter));

            _outputters.Add(outputter);
        }

        /// <summary>
        /// Adds parameter for data base manager. Method knows where this element should place.
        /// </summary>
        /// <param name="dataBaseManagerParameter">
        /// XML element which represents parameter for <see cref="DAL.DataBaseManager" />.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dataBaseManagerParameter" /> is <c>null</c>.
        /// </exception>
        public void AddDataBaseManagerParameter(XElement dataBaseManagerParameter)
        {
            dataBaseManagerParameter.ThrowIfNull(nameof(dataBaseManagerParameter));

            _dataBaseManagerParameters.Add(dataBaseManagerParameter);
        }

        /// <summary>
        /// Adds new element for data base manager part of XML configuration.
        /// </summary>
        /// <param name="repository">XML element to add.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="repository" /> is <c>null</c>.
        /// </exception>
        public void AddRepository(XElement repository)
        {
            repository.ThrowIfNull(nameof(repository));

            _repositories.Add(repository);
        }

        /// <summary>
        /// Sets service type which used by builder for make proper shell instance.
        /// </summary>
        /// <param name="serviceType">Type of the service pipeline.</param>
        public void SetServiceType(ServiceType serviceType)
        {
            _result.ServiceType = serviceType;
        }

        /// <summary>
        /// Returns current version of XML configuration as config class.
        /// </summary>
        /// <returns>Created XML configuration.</returns>
        public ConfigurationXml GetResult()
        {
            _result.ShellConfig.MessageHandler.MessageHandlerParameters = 
                _messageHandlerParameters.ToArray();

            _result.ShellConfig.InputManager.InputManagerParameters = 
                _inputManagerParameters.ToArray();
            _result.ShellConfig.InputManager.Inputters = _inputters.ToArray();

            _result.ShellConfig.CrawlersManager.CrawlersManagerParameters = 
                _crawlersManagerParameters.ToArray();
            _result.ShellConfig.CrawlersManager.Crawlers = _crawlers.ToArray();

            _result.ShellConfig.AppraisersManager.AppraisersManagerParameters = 
                _appraisersManagerParameters.ToArray();
            _result.ShellConfig.AppraisersManager.Appraisers = _appraisers.ToArray();

            _result.ShellConfig.OutputManager.OutputManagerParameters = 
                _outputManagerParameters.ToArray();
            _result.ShellConfig.OutputManager.Outputters = _outputters.ToArray();

            _result.ShellConfig.DataBaseManager.DataBaseManagerParameters = 
                _dataBaseManagerParameters.ToArray();
            _result.ShellConfig.DataBaseManager.Repositories = _repositories.ToArray();

            _logger.Info("Creates new configuration for shell instance.");
            return _result;
        }

        /// <summary>
        /// Returns current version of XML configuration as serialized value.
        /// </summary>
        /// <returns>Created XML configuration.</returns>
        public XDocument GetResultAsXDocument()
        {
            var xmlConfig = GetResult();

            _logger.Info("Creates new configuration for shell instance.");
            return TransformConfigToXDocument(xmlConfig);
        }
    }
}
