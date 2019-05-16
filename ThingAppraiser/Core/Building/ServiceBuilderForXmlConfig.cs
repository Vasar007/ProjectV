using System;
using System.Xml.Linq;
using ThingAppraiser.Logging;

namespace ThingAppraiser.Core.Building
{
    /// <summary>
    /// Provides methods to create service classes (sequential) instances with parameters from XML 
    /// config.
    /// </summary>
    public sealed class ServiceBuilderForXmlConfig : ServiceBuilderBase
    {
        /// <summary>
        /// Logger instance for current class.
        /// </summary>
        private static readonly LoggerAbstraction _logger =
            LoggerAbstraction.CreateLoggerInstanceFor<ServiceBuilderForXmlConfig>();


        /// <summary>
        /// Creates service (sequential) builder to interact with XML config.
        /// </summary>
        public ServiceBuilderForXmlConfig()
        {
        }

        /// <summary>
        /// Creates message handler (sequential) instance depend on parameter value (could be get 
        /// from config).
        /// </summary>
        /// <param name="messageHandlerElement">Element from XML config.</param>
        /// <returns>Fully initialized instance of message handler class.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="messageHandlerElement" /> isn't specified in XML config.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="messageHandlerElement" /> or its attribute with handler name is 
        /// <c>null</c>.
        /// </exception>
        public Communication.IMessageHandler CreateMessageHandler(XElement messageHandlerElement)
        {
            messageHandlerElement.ThrowIfNull(nameof(messageHandlerElement));
            var handlerElement = messageHandlerElement.Attribute(_messageHandlerTypeParameterName);
            handlerElement.ThrowIfNull(nameof(handlerElement));

            switch (handlerElement.Value)
            {
                case _consoleMessageHandlerParameterName:
                {
                    var messageHandlerParametersElement = XDocumentParser.FindSubelement(
                        messageHandlerElement, _setUnicodeParameterName
                    );
                    var setUnicode = XDocumentParser.GetElementValue<bool>(
                        messageHandlerParametersElement
                    );

                    return new Communication.ConsoleMessageHandler(setUnicode);
                }

                default:
                {
                    var ex = new ArgumentOutOfRangeException(
                        nameof(messageHandlerElement), messageHandlerElement,
                        "Couldn't recognize message handler type specified in XML config."
                    );
                    _logger.Error(ex, "Passed incorrect data to method: " +
                                       $"{handlerElement.Value}");
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Creates inputter (sequential) instance depend on parameter value (could be get from 
        /// config).
        /// </summary>
        /// <param name="inputterElement">Element from XML config.</param>
        /// <returns>Fully initialized instance of inputter interface.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="inputterElement" /> isn't specified in config.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="inputterElement" /> is <c>null</c>.
        /// </exception>
        public IO.Input.IInputter CreateInputter(XElement inputterElement)
        {
            inputterElement.ThrowIfNull(nameof(inputterElement));

            switch (inputterElement.Name.LocalName)
            {
                case _localFileParameterName:
                {
                    string fileReaderName = XDocumentParser.GetAttributeValue(
                        inputterElement, _fileReaderParameterName + _localFileParameterName
                    );

                    IO.Input.IFileReader fileReader = CreateFileReader(fileReaderName);

                    return new IO.Input.LocalFileReader(fileReader);
                }

                case _googleDriveParameterName:
                {
                    string fileReaderName = XDocumentParser.GetAttributeValue(
                        inputterElement, _fileReaderParameterName + _googleDriveParameterName
                    );

                    IO.Input.IFileReader fileReader = CreateFileReader(fileReaderName);

                    return new IO.Input.GoogleDriveReader(_driveService, fileReader);
                }

                default:
                {
                    var ex = new ArgumentOutOfRangeException(
                        nameof(inputterElement), inputterElement,
                        "Couldn't recognize input type specified in XML config."
                    );
                    _logger.Error(ex, "Passed incorrect data to method: " +
                                       $"{inputterElement.Name.LocalName}");
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Creates crawler (sequential) instance depend on parameter value (could be get from 
        /// config).
        /// </summary>
        /// <param name="crawlerElement">Element from XML config.</param>
        /// <returns>Fully initialized instance of crawler class.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="crawlerElement" /> isn't specified in config.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="crawlerElement" /> is <c>null</c>.
        /// </exception>
        public Crawlers.Crawler CreateCrawler(XElement crawlerElement)
        {
            crawlerElement.ThrowIfNull(nameof(crawlerElement));

            switch (crawlerElement.Name.LocalName)
            {
                case _tmdbCrawlerParameterName:
                {
                    string apiKey = XDocumentParser.GetAttributeValue(
                        crawlerElement, _tmdbApiKeyParameterName
                    );
                    var maxRetryCount = XDocumentParser.GetAttributeValue<int>(
                        crawlerElement, _tmdbMaxRetryCountParameterName
                    );

                    return new Crawlers.TmdbCrawler(apiKey, maxRetryCount);
                }

                case _steamCrawlerParameterName:
                {
                    string apiKey = XDocumentParser.GetAttributeValue(
                        crawlerElement, _steamApiKeyParameterName
                    );

                    return new Crawlers.SteamCrawler(apiKey);
                }

                default:
                {
                    var ex = new ArgumentOutOfRangeException(
                        nameof(crawlerElement), crawlerElement,
                        "Couldn't recognize crawler type specified in XML config."
                    );
                    _logger.Error(ex, "Passed incorrect data to method: " +
                                       $"{crawlerElement.Name.LocalName}");
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Creates appraiser (sequential) instance depend on parameter value (could be get from 
        /// config).
        /// </summary>
        /// <param name="appraiserElement">Element from XML config.</param>
        /// <returns>Fully initialized instance of appraiser class.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="appraiserElement" /> isn't specified in config.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="appraiserElement" /> is <c>null</c>.
        /// </exception>
        public Appraisers.Appraiser CreateAppraiser(XElement appraiserElement)
        {
            appraiserElement.ThrowIfNull(nameof(appraiserElement));

            switch (appraiserElement.Name.LocalName)
            {
                case _appraiserTmdbParameterName:
                {
                    return new Appraisers.TmdbAppraiser();
                }

                case _fuzzyAppraiserTmdbParameterName:
                {
                    return new Appraisers.FuzzyTmdbAppraiser();
                }

                case _steamAppraiserParameterName:
                {
                    return new Appraisers.SteamAppraiser();
                }

                default:
                {
                    var ex = new ArgumentOutOfRangeException(
                        nameof(appraiserElement), appraiserElement,
                        "Couldn't recognize appraiser type specified in XML config."
                    );
                    _logger.Error(ex, "Passed incorrect data to method: " +
                                       $"{appraiserElement.Name.LocalName}");
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Creates outputter (sequential) instance depend on parameter value (could be get from 
        /// config).
        /// </summary>
        /// <param name="outputterElement">Element from XML config.</param>
        /// <returns>Fully initialized instance of outputter interface.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="outputterElement" /> isn't specified in config.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="outputterElement" /> is <c>null</c>.
        /// </exception>
        public IO.Output.IOutputter CreateOutputter(XElement outputterElement)
        {
            outputterElement.ThrowIfNull(nameof(outputterElement));

            switch (outputterElement.Name.LocalName)
            {
                case _localFileParameterName:
                {
                    return new IO.Output.LocalFileWriter();
                }

                case _googleDriveParameterName:
                {
                    return new IO.Output.GoogleDriveWriter(_driveService);
                }

                default:
                {
                    var ex = new ArgumentOutOfRangeException(
                        nameof(outputterElement), outputterElement,
                        "Couldn't recognize output type specified in XML config."
                    );
                    _logger.Error(ex, "Passed incorrect data to method: " +
                                       $"{outputterElement.Name.LocalName}");
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Creates repository (sequential) instance depend on parameter value (could be get from 
        /// config).
        /// </summary>
        /// <param name="repositoryElement">Element from XML config.</param>
        /// <param name="storageSettings">
        /// Storage settings for repositrory (at least contain connection string).
        /// </param>
        /// <returns>Fully initialized instance of repository interface.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="repositoryElement" /> isn't specified in config.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="repositoryElement" /> or <paramref name="storageSettings" /> is 
        /// <c>null</c>.
        /// </exception>
        public DAL.Repositories.IDataRepository CreateRepository(XElement repositoryElement,
            DAL.DataStorageSettings storageSettings)
        {
            repositoryElement.ThrowIfNull(nameof(repositoryElement));
            storageSettings.ThrowIfNull(nameof(storageSettings));

            switch (repositoryElement.Name.LocalName)
            {
                case _basicInfoRepositoryParameterName:
                {
                    return new DAL.Repositories.BasicInfoRepository(storageSettings);
                }

                case _tmdbMovieRepositoryParameterName:
                {
                    return new DAL.Repositories.TmdbMovieRepository(storageSettings);
                }

                default:
                {
                    var ex = new ArgumentOutOfRangeException(
                        nameof(repositoryElement), repositoryElement,
                        "Couldn't recognize output type specified in XML config."
                    );
                    _logger.Error(ex, "Passed incorrect data to method: " +
                                       $"{repositoryElement.Name.LocalName}");
                    throw ex;
                }
            }
        }
    }
}
