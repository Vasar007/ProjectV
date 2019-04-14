using System;
using System.IO;
using System.Threading;
using System.Xml.Linq;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using ThingAppraiser.Logging;

namespace ThingAppraiser.Core.Building
{
    /// <summary>
    /// Represents methods to create classes for service depend on config parameters.
    /// </summary>
    public static class SServiceBuilder
    {
        /// <summary>
        /// Logger instance for current class.
        /// </summary>
        private static readonly CLoggerAbstraction s_logger =
            CLoggerAbstraction.CreateLoggerInstanceWithName(nameof(SServiceBuilder));

        /// <summary>
        /// Provides thread-safe service to interact with Google Drive.
        /// </summary>
        private static readonly DriveService s_driveService = CreateDriveService();

        /// <summary>
        /// Attribute value for console message handler.
        /// </summary>
        private const String _consoleMessageHandlerParameterName = "ConsoleMessageHandler";

        /// <summary>
        /// Attribute value for local file handlers.
        /// </summary>
        private const String _localFileParameterName = "LocalFile";

        /// <summary>
        /// Attribute value for Google Drive handlers.
        /// </summary>
        private const String _googleDriveParameterName = "GoogleDrive";

        /// <summary>
        /// Attribute value for TMDB crawler.
        /// </summary>
        private const String _crawlerTMDBParameterName = "CrawlerTMDB";

        /// <summary>
        /// Attribute value for TMDB appraiser.
        /// </summary>
        private const String _appraiserTMDBParameterName = "AppraiserTMDB";

        /// <summary>
        /// Attribute value for TMDB appraiser which based on Fuzzy Logic Toolbox.
        /// </summary>
        private const String _fuzzyAppraiserTMDBParameterName = "FuzzyAppraiserTMDB";

        /// <summary>
        /// Attribute value for simple file reader.
        /// </summary>
        private const String _simpleFileReaderParameterName = "Simple";

        /// <summary>
        /// Attribute value for filter file reader.
        /// </summary>
        private const String _filterFileReaderParameterName = "Filter";

        /// <summary>
        /// Attribute name for message handler.
        /// </summary>
        private static readonly String _messageHandlerTypeParameterName = "MessageHandlerType";

        /// <summary>
        /// Attribute name for flag which represents changing default encoding of message handler.
        /// </summary>
        private static readonly String _setUnicodeParameterName = "ConsoleMessageHandlerSetUnicode";

        /// <summary>
        /// Attribute name for inputters to define instance of file reader.
        /// </summary>
        private static readonly String _fileReaderParameterName = "FileReader";

        /// <summary>
        /// Attribute name for API key of TMDB crawler.
        /// </summary>
        private static readonly String _APIKeyTMDBParameterName = "APIKeyTMDB";

        /// <summary>
        /// Attribute name for search URL of TMDB crawler.
        /// </summary>
        private static readonly String _searchUrlTMDBParameterName = "SearchUrlTMDB";

        /// <summary>
        /// Attribute name for configuration URL of TMDB crawler.
        /// </summary>
        private static readonly String _configurationUrlTMDBParameterName = "ConfigurationUrlTMDB";

        /// <summary>
        /// Attribute name for requests per time of TMDB crawler.
        /// </summary>
        private static readonly String _requestsPerTimeTMDBParameterName = "RequestsPerTimeTMDB";

        /// <summary>
        /// Attribute name for good status code of TMDB crawler.
        /// </summary>
        private static readonly String _goodStatusCodeTMDBParameterName = "GoodStatusCodeTMDB";

        /// <summary>
        /// Attribute name for limit attempts of TMDB crawler.
        /// </summary>
        private static readonly String _limitAttemptsTMDBParameterName = "LimitAttemptsTMDB";

        /// <summary>
        /// Attribute name for operation timeout of TMDB crawler.
        /// </summary>
        private static readonly String _millisecondsTimeoutTMDBParameterName =
            "MillisecondsTimeoutTMDB";


        /// <summary>
        /// Creates message handler instance depend on parameter value (could be read from config 
        /// file).
        /// </summary>
        /// <param name="messageHandlerName">Name of the message handler class to create.</param>
        /// <returns>Fully initialized instance of message handler class.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="messageHandlerName">messageHandlerName</paramref> isn't specified in
        /// method.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="messageHandlerName">messageHandlerName</paramref> is <c>null</c> or
        /// presents empty string.
        /// </exception>
        public static Communication.IMessageHandler CreateMessageHandlerWithConfigParameters(
            String messageHandlerName)
        {
            messageHandlerName.ThrowIfNullOrEmpty(nameof(messageHandlerName));

            switch (messageHandlerName)
            {
                case _consoleMessageHandlerParameterName:
                {
                    Boolean setUnicode = SConfigParser.GetValueByParameterKey<Boolean>(
                        _setUnicodeParameterName
                    );
                    return new Communication.CConsoleMessageHandler(setUnicode);
                }

                default:
                {
                    var ex = new ArgumentOutOfRangeException(
                        nameof(messageHandlerName), messageHandlerName,
                        "Couldn't recognize message handler type."
                    );
                    s_logger.Error(ex, $"Passed incorrect data to method: {messageHandlerName}");
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Creates inputter instance depend on parameter value (could be read from config file).
        /// </summary>
        /// <param name="inputterName">Name of the inputter.</param>
        /// <returns>Fully initialized instance of inputter interface.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="inputterName">inputType</paramref> isn't specified in method.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="inputterName">inputterName</paramref> is <c>null</c> or presents empty
        /// string.
        /// </exception>
        public static IO.Input.IInputter CreateInputterWithConfigParameters(String inputterName)
        {
            inputterName.ThrowIfNullOrEmpty(nameof(inputterName));

            switch (inputterName)
            {
                case _localFileParameterName:
                {
                    String fileReaderName = SConfigParser.GetValueByParameterKey(
                        _fileReaderParameterName + _localFileParameterName
                    );

                    IO.Input.IFileReader fileReader = CreateFileReaderFromName(fileReaderName);

                    return new IO.Input.CLocalFileReader(fileReader);
                }

                case _googleDriveParameterName:
                {
                    String fileReaderName = SConfigParser.GetValueByParameterKey(
                        _fileReaderParameterName + _googleDriveParameterName
                    );

                    IO.Input.IFileReader fileReader = CreateFileReaderFromName(fileReaderName);

                    return new IO.Input.CGoogleDriveReader(s_driveService, fileReader);
                }

                default:
                {
                    var ex = new ArgumentOutOfRangeException(nameof(inputterName), inputterName,
                                                             "Couldn't recognize input type.");
                    s_logger.Error(ex, $"Passed incorrect data to method: {inputterName}");
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Creates crawler instance depend on parameter value (could be read from config file).
        /// </summary>
        /// <param name="crawlerName">Name of the crawler class to create.</param>
        /// <returns>Fully initialized instance of crawler class.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="crawlerName">crawlerName</paramref> isn't specified in method.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="crawlerName">crawlerName</paramref> is <c>null</c> or presents empty
        /// string.
        /// </exception>
        public static Crawlers.CCrawler CreateCrawlerWithConfigParameters(String crawlerName)
        {
            crawlerName.ThrowIfNullOrEmpty(nameof(crawlerName));

            switch (crawlerName)
            {
                case _crawlerTMDBParameterName:
                {
                    String APiKey = SConfigParser.GetValueByParameterKey(
                        _APIKeyTMDBParameterName
                    );
                    String searchUrl = SConfigParser.GetValueByParameterKey(
                        _searchUrlTMDBParameterName
                    );
                    String configurationUrl = SConfigParser.GetValueByParameterKey(
                        _configurationUrlTMDBParameterName
                    );
                    Int32 requestsPerTime = SConfigParser.GetValueByParameterKey<Int32>(
                        _requestsPerTimeTMDBParameterName
                    );
                    String goodStatus = SConfigParser.GetValueByParameterKey(
                        _goodStatusCodeTMDBParameterName
                    );
                    Int32 limitAttempts = SConfigParser.GetValueByParameterKey<Int32>(
                        _limitAttemptsTMDBParameterName
                    );
                    Int32 millisecondsTimeout = SConfigParser.GetValueByParameterKey<Int32>(
                        _millisecondsTimeoutTMDBParameterName
                    );

                    return new Crawlers.CCrawlerTMDB(APiKey, searchUrl, configurationUrl,
                                                     requestsPerTime, goodStatus, limitAttempts,
                                                     millisecondsTimeout);
                }

                default:
                { 
                    var ex = new ArgumentOutOfRangeException(nameof(crawlerName), crawlerName,
                                                             "Couldn't recognize crawler type.");
                    s_logger.Error(ex, $"Passed incorrect data to method: {crawlerName}");
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Creates appraiser instance depend on parameter value (could be read from config file).
        /// </summary>
        /// <param name="appraiserName">Name of the appraiser class to create.</param>
        /// <returns>Fully initialized instance of appraiser class.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="appraiserName">appraiserName</paramref> isn't specified in method.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="appraiserName">appraiserName</paramref> is <c>null</c> or presents empty
        /// string.
        /// </exception>
        public static Appraisers.CAppraiser CreateAppraiserWithConfigParameters(
            String appraiserName)
        {
            appraiserName.ThrowIfNullOrEmpty(appraiserName);

            switch (appraiserName)
            {
                case _appraiserTMDBParameterName:
                {
                    return new Appraisers.CAppraiserTMDB();
                }

                case _fuzzyAppraiserTMDBParameterName:
                {
                    return new Appraisers.CFuzzyAppraiserTMDB();
                }

                default:
                {
                    var ex = new ArgumentOutOfRangeException(nameof(appraiserName), appraiserName,
                                                             "Couldn't recognize appraiser.");
                    s_logger.Error(ex, $"Passed incorrect data to method: {appraiserName}");
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Creates outputter instance depend on parameter value (could be read from config file).
        /// </summary>
        /// <param name="outputterName">Name of the outputter.</param>
        /// <returns>Fully initialized instance of outputter interface.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="outputterName">outputType</paramref> isn't specified in method.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="outputterName">outputterName</paramref> is <c>null</c> or presents empty
        /// string.
        /// </exception>
        public static IO.Output.IOutputter CreateOutputterWithConfigParameters(String outputterName)
        {
            outputterName.ThrowIfNullOrEmpty(nameof(outputterName));

            switch (outputterName)
            {
                case _localFileParameterName:
                {
                    return new IO.Output.CLocalFileWriter();
                }

                case _googleDriveParameterName:
                {
                    return new IO.Output.CGoogleDriveWriter(s_driveService);
                }

                default:
                {
                    var ex = new ArgumentOutOfRangeException(nameof(outputterName), outputterName,
                                                             "Couldn't recognize output type.");
                    s_logger.Error(ex, $"Passed incorrect data to method: {outputterName}");
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Creates message handler instance depend on parameter value (could be get from config).
        /// </summary>
        /// <param name="messageHandlerElement">Element from XML config.</param>
        /// <returns>Fully initialized instance of message handler class.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="messageHandlerElement">messageHandlerElement</paramref> isn't specified
        /// in XML config.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="messageHandlerElement">messageHandlerElement</paramref> or its
        /// attribute with handler name is <c>null</c>.
        /// </exception>
        public static Communication.IMessageHandler CreateMessageHandlerWithXMLParameters(
            XElement messageHandlerElement)
        {
            messageHandlerElement.ThrowIfNull(nameof(messageHandlerElement));
            var handlerElement = messageHandlerElement.Attribute(_messageHandlerTypeParameterName);
            handlerElement.ThrowIfNull(nameof(handlerElement));

            switch (handlerElement.Value)
            {
                case _consoleMessageHandlerParameterName:
                {
                    Boolean setUnicode = CXDocumentParser.GetAttributeValue<Boolean>(
                        messageHandlerElement, _setUnicodeParameterName
                    );

                    return new Communication.CConsoleMessageHandler(setUnicode);
                }

                default:
                {
                    var ex = new ArgumentOutOfRangeException(
                        nameof(messageHandlerElement), messageHandlerElement,
                        "Couldn't recognize message handler type specified in XML config."
                    );
                    s_logger.Error(ex, "Passed incorrect data to method: " +
                                       $"{handlerElement.Value}");
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Creates inputter instance depend on parameter value (could be get from config).
        /// </summary>
        /// <param name="inputterElement">Element from XML config.</param>
        /// <returns>Fully initialized instance of inputter interface.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="inputterElement">inputterElement</paramref> isn't specified in config.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="inputterElement">inputterElement</paramref> is <c>null</c>.
        /// </exception>
        public static IO.Input.IInputter CreateInputterWithXMLParameters(XElement inputterElement)
        {
            inputterElement.ThrowIfNull(nameof(inputterElement));

            switch (inputterElement.Name.LocalName)
            {
                case _localFileParameterName:
                {
                    String fileReaderName = CXDocumentParser.GetAttributeValue(
                        inputterElement, _fileReaderParameterName + _localFileParameterName
                    );

                    IO.Input.IFileReader fileReader = CreateFileReaderFromName(fileReaderName);

                    return new IO.Input.CLocalFileReader(fileReader);
                }

                case _googleDriveParameterName:
                {
                    String fileReaderName = CXDocumentParser.GetAttributeValue(
                        inputterElement, _fileReaderParameterName + _googleDriveParameterName
                    );

                    IO.Input.IFileReader fileReader = CreateFileReaderFromName(fileReaderName);

                    return new IO.Input.CGoogleDriveReader(s_driveService, fileReader);
                }

                default:
                {
                    var ex = new ArgumentOutOfRangeException(
                        nameof(inputterElement), inputterElement,
                        "Couldn't recognize input type specified in XML config."
                    );
                    s_logger.Error(ex, "Passed incorrect data to method: " +
                                       $"{inputterElement.Name.LocalName}");
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Creates crawler instance depend on parameter value (could be get from config).
        /// </summary>
        /// <param name="crawlerElement">Element from XML config.</param>
        /// <returns>Fully initialized instance of crawler class.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="crawlerElement">crawlerElement</paramref> isn't specified in config.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="crawlerElement">crawlerElement</paramref> is <c>null</c>.
        /// </exception>
        public static Crawlers.CCrawler CreateCrawlerWithXMLParameters(XElement crawlerElement)
        {
            crawlerElement.ThrowIfNull(nameof(crawlerElement));

            switch (crawlerElement.Name.LocalName)
            {
                case _crawlerTMDBParameterName:
                {
                    String APiKey = CXDocumentParser.GetAttributeValue(
                        crawlerElement, _APIKeyTMDBParameterName
                    );
                    String searchUrl = CXDocumentParser.GetAttributeValue(
                        crawlerElement, _searchUrlTMDBParameterName
                    );
                    String configurationUrl = CXDocumentParser.GetAttributeValue(
                        crawlerElement, _configurationUrlTMDBParameterName
                    );
                    Int32 requestsPerTime = CXDocumentParser.GetAttributeValue<Int32>(
                        crawlerElement, _requestsPerTimeTMDBParameterName
                    );
                    String goodStatus = CXDocumentParser.GetAttributeValue(
                        crawlerElement, _goodStatusCodeTMDBParameterName
                    );
                    Int32 limitAttempts = CXDocumentParser.GetAttributeValue<Int32>(
                        crawlerElement, _limitAttemptsTMDBParameterName
                    );
                    Int32 millisecondsTimeout = CXDocumentParser.GetAttributeValue<Int32>(
                        crawlerElement, _millisecondsTimeoutTMDBParameterName
                    );

                    return new Crawlers.CCrawlerTMDB(APiKey, searchUrl, configurationUrl,
                                                     requestsPerTime, goodStatus, limitAttempts,
                                                     millisecondsTimeout);
                }

                default:
                {
                    var ex = new ArgumentOutOfRangeException(
                        nameof(crawlerElement), crawlerElement,
                        "Couldn't recognize crawler type specified in XML config."
                    );
                    s_logger.Error(ex, "Passed incorrect data to method: " +
                                       $"{crawlerElement.Name.LocalName}");
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Creates appraiser instance depend on parameter value (could be get from config).
        /// </summary>
        /// <param name="appraiserElement">Element from XML config.</param>
        /// <returns>Fully initialized instance of appraiser class.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="appraiserElement">appraiserElement</paramref> isn't specified in config.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="appraiserElement">appraiserElement</paramref> is <c>null</c>.
        /// </exception>
        public static Appraisers.CAppraiser CreateAppraiserWithXMLParameters(
            XElement appraiserElement)
        {
            appraiserElement.ThrowIfNull(nameof(appraiserElement));

            switch (appraiserElement.Name.LocalName)
            {
                case _appraiserTMDBParameterName:
                {
                    return new Appraisers.CAppraiserTMDB();
                }

                case _fuzzyAppraiserTMDBParameterName:
                {
                    return new Appraisers.CFuzzyAppraiserTMDB();
                }

                default:
                {
                    var ex = new ArgumentOutOfRangeException(
                        nameof(appraiserElement), appraiserElement,
                        "Couldn't recognize appraiser type specified in XML config."
                    );
                    s_logger.Error(ex, "Passed incorrect data to method: " +
                                       $"{appraiserElement.Name.LocalName}");
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Creates outputter instance depend on parameter value (could be get from config).
        /// </summary>
        /// <param name="outputterElement">Element from XML config.</param>
        /// <returns>Fully initialized instance of outputter interface.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="outputterElement">outputterElement</paramref> isn't specified in config.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="outputterElement">outputterElement</paramref> is <c>null</c>.
        /// </exception>
        public static IO.Output.IOutputter CreateOutputterWithXMLParameters(
            XElement outputterElement)
        {
            outputterElement.ThrowIfNull(nameof(outputterElement));

            switch (outputterElement.Name.LocalName)
            {
                case _localFileParameterName:
                {
                    return new IO.Output.CLocalFileWriter();
                }

                case _googleDriveParameterName:
                {
                    return new IO.Output.CGoogleDriveWriter(s_driveService);
                }

                default:
                {
                    var ex = new ArgumentOutOfRangeException(
                        nameof(outputterElement), outputterElement,
                        "Couldn't recognize output type specified in XML config."
                    );
                    s_logger.Error(ex, "Passed incorrect data to method: " +
                                       $"{outputterElement.Name.LocalName}");
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Creates file reader instance depend on parameter value (could be read from config file
        /// or XML document).
        /// </summary>
        /// <param name="fileReaderName">Name of the file reader class to create.</param>
        /// <returns>Fully initialized instance of file reader class.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="fileReaderName">fileReaderName</paramref> isn't specified in
        /// method.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="fileReaderName">fileReaderName</paramref> is <c>null</c> or presents
        /// empty string.
        /// </exception>
        public static IO.Input.IFileReader CreateFileReaderFromName(String fileReaderName)
        {
            fileReaderName.ThrowIfNullOrEmpty(nameof(fileReaderName));

            switch (fileReaderName)
            {
                case _simpleFileReaderParameterName:
                {
                    return new IO.Input.CSimpleFileReader();
                }

                case _filterFileReaderParameterName:
                {
                    return new IO.Input.CFilterFileReader();
                }

                default:
                {
                    var ex = new ArgumentOutOfRangeException(
                        nameof(fileReaderName), fileReaderName,
                        "Couldn't recognize file reader type."
                    );
                    s_logger.Error(ex, $"Passed incorrect data to method: {fileReaderName}");
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Creates Google Drive Service instance with default parameters.
        /// </summary>
        /// <returns>Initialized Drive Service instance.</returns>
        /// <remarks>
        /// There is no need to create more than one instance of <see cref="DriveService" /> class.
        /// </remarks>
        private static DriveService CreateDriveService()
        {
            UserCredential credential;
            using (var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                const String credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    IO.CGoogleDriveWorker.Scopes,
                    IO.CGoogleDriveWorker.ApplicationName,
                    CancellationToken.None,
                    new FileDataStore(credPath, true)
                ).Result;

                s_logger.Info($"Credential file saved to: \"{credPath}\"");
            }

            // Create Drive API service.
            return new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = IO.CGoogleDriveWorker.ApplicationName
            });
        }
    }
}
