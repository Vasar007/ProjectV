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
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor<ServiceBuilderForXmlConfig>();


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

            _logger.Info("Creating message handler.");

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
                    throw new ArgumentOutOfRangeException(
                        nameof(messageHandlerElement), messageHandlerElement,
                        "Couldn't recognize message handler type specified in XML config."
                    );
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

            _logger.Info("Creating inputter.");

            switch (inputterElement.Name.LocalName)
            {
                case _localFileParameterName:
                {
                    string fileReaderName = XDocumentParser.GetAttributeValue(
                        inputterElement, _fileReaderParameterName + _localFileParameterName
                    );

                    IO.Input.File.IFileReader fileReader = CreateFileReader(fileReaderName);

                    return new IO.Input.File.LocalFileReader(fileReader);
                }

                case _googleDriveParameterName:
                {
                    string fileReaderName = XDocumentParser.GetAttributeValue(
                        inputterElement, _fileReaderParameterName + _googleDriveParameterName
                    );

                    IO.Input.File.IFileReader fileReader = CreateFileReader(fileReaderName);

                    return new IO.Input.GoogleDrive.GoogleDriveReader(_driveService, fileReader);
                }

                default:
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(inputterElement), inputterElement,
                        "Couldn't recognize input type specified in XML config."
                    );
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

            _logger.Info("Creating crawler.");

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

                    return new Crawlers.Tmdb.TmdbCrawler(apiKey, maxRetryCount);
                }

                case _omdbCrawlerParameterName:
                {
                    string apiKey = XDocumentParser.GetAttributeValue(
                        crawlerElement, _omdbApiKeyParameterName
                    );

                    return new Crawlers.Omdb.OmdbCrawler(apiKey);
                }

                case _steamCrawlerParameterName:
                {
                    string apiKey = XDocumentParser.GetAttributeValue(
                        crawlerElement, _steamApiKeyParameterName
                    );

                    return new Crawlers.Steam.SteamCrawler(apiKey);
                }

                default:
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(crawlerElement), crawlerElement,
                        "Couldn't recognize crawler type specified in XML config."
                    );
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

            _logger.Info("Creating appraiser.");

            switch (appraiserElement.Name.LocalName)
            {
                case _appraiserTmdbParameterName:
                {
                    return new Appraisers.MoviesRating.Tmdb.TmdbAppraiser();
                }

                case _fuzzyAppraiserTmdbParameterName:
                {
                    return new Appraisers.MoviesRating.Tmdb.FuzzyTmdbAppraiser();
                }

                case _appraiserOmdbParameterName:
                {
                    return new Appraisers.MoviesRating.Omdb.OmdbAppraiser();
                }

                case _steamAppraiserParameterName:
                {
                    return new Appraisers.GameRating.Steam.SteamAppraiser();
                }

                default:
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(appraiserElement), appraiserElement,
                        "Couldn't recognize appraiser type specified in XML config."
                    );
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

            _logger.Info("Creating outputter.");

            switch (outputterElement.Name.LocalName)
            {
                case _localFileParameterName:
                {
                    return new IO.Output.File.LocalFileWriter();
                }

                case _googleDriveParameterName:
                {
                    return new IO.Output.GoogleDrive.GoogleDriveWriter(_driveService);
                }

                default:
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(outputterElement), outputterElement,
                        "Couldn't recognize output type specified in XML config."
                    );
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

            _logger.Info("Creating reppository.");

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
                    throw new ArgumentOutOfRangeException(
                        nameof(repositoryElement), repositoryElement,
                        "Couldn't recognize output type specified in XML config."
                    );
                }
            }
        }
    }
}
