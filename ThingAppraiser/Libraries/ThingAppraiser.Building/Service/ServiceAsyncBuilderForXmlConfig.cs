using System;
using System.Xml.Linq;
using ThingAppraiser.Logging;
using ThingAppraiser.Models.Data;

namespace ThingAppraiser.Building.Service
{
    /// <summary>
    /// Provides methods to create service classes (async) instances with parameters from XML 
    /// config.
    /// </summary>
    public sealed class ServiceAsyncBuilderForXmlConfig : ServiceBuilderBase
    {
        /// <summary>
        /// Logger instance for current class.
        /// </summary>
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor<ServiceAsyncBuilderForXmlConfig>();


        /// <summary>
        /// Creates service (async) builder to interact with XML config.
        /// </summary>
        public ServiceAsyncBuilderForXmlConfig()
        {
        }

        /// <summary>
        /// Creates message (sequential) handler instance depend on parameter value (could be get 
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
                    XElement? messageHandlerParametersElement = XDocumentParser.FindSubelement(
                        messageHandlerElement, _setUnicodeParameterName
                    );

                    if (messageHandlerParametersElement is null)
                    {
                        throw new ArgumentException(
                            "Invalid structure of XML document: cannot find message handler " +
                            "parameters block.",
                            nameof(messageHandlerElement)
                        );
                    }

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
        /// Creates inputter (async) instance depend on parameter value (could be get from config).
        /// </summary>
        /// <param name="inputterElement">Element from XML config.</param>
        /// <returns>Fully initialized instance of inputter interface.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="inputterElement" /> isn't specified in config.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="inputterElement" /> is <c>null</c>.
        /// </exception>
        public IO.Input.IInputterAsync CreateInputter(XElement inputterElement)
        {
            inputterElement.ThrowIfNull(nameof(inputterElement));

            _logger.Info("Creating intputter.");

            switch (inputterElement.Name.LocalName)
            {
                case _localFileParameterName:
                {
                    string fileReaderName = XDocumentParser.GetAttributeValue(
                        inputterElement, _fileReaderParameterName + _localFileParameterName
                    );

                    IO.Input.File.IFileReaderAsync fileReader =
                        CreateFileReaderAsync(fileReaderName);

                    return new IO.Input.File.LocalFileReaderAsync(fileReader);
                }

                case _googleDriveParameterName:
                {
                    throw new NotImplementedException("Now GoogleDrive isn't supported.");
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
        /// Creates crawler (async) instance depend on parameter value (could be get from config).
        /// </summary>
        /// <param name="crawlerElement">Element from XML config.</param>
        /// <returns>Fully initialized instance of crawler class.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="crawlerElement" /> isn't specified in config.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="crawlerElement" /> is <c>null</c>.
        /// </exception>
        public Crawlers.CrawlerAsync CreateCrawler(XElement crawlerElement)
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

                    return new Crawlers.Movie.Tmdb.TmdbCrawlerAsync(apiKey, maxRetryCount);
                }

                case _omdbCrawlerParameterName:
                {
                    string apiKey = XDocumentParser.GetAttributeValue(
                        crawlerElement, _omdbApiKeyParameterName
                    );

                    return new Crawlers.Movie.Omdb.OmdbCrawlerAsync(apiKey);
                }

                case _steamCrawlerParameterName:
                {
                    string apiKey = XDocumentParser.GetAttributeValue(
                        crawlerElement, _steamApiKeyParameterName
                    );

                    return new Crawlers.Game.Steam.SteamCrawlerAsync(apiKey);
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
        /// Creates appraiser (async) instance depend on parameter value (could be get from config).
        /// </summary>
        /// <param name="appraiserElement">Element from XML config.</param>
        /// <returns>Fully initialized instance of appraiser class.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="appraiserElement" /> isn't specified in config.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="appraiserElement" /> is <c>null</c>.
        /// </exception>
        public Appraisers.IAppraiserAsync CreateAppraiser(XElement appraiserElement)
        {
            appraiserElement.ThrowIfNull(nameof(appraiserElement));

            _logger.Info("Creating appraiser.");

            switch (appraiserElement.Name.LocalName)
            {
                case _appraiserTmdbParameterName:
                {
                    var appraisal = new Appraisers.Appraisals.Movie.Tmdb.TmdbCommonAppraisal();

                    return new Appraisers.AppraiserAsync<TmdbMovieInfo>(appraisal);
                }

                case _fuzzyAppraiserTmdbParameterName:
                {
                    var appraisal = new Appraisers.Appraisals.Movie.Tmdb.TmdbFuzzyAppraisal();

                    return new Appraisers.AppraiserAsync<TmdbMovieInfo>(appraisal);
                }

                case _appraiserOmdbParameterName:
                {
                    var appraisal = new Appraisers.Appraisals.Movie.Omdb.OmdbCommonAppraisal();

                    return new Appraisers.AppraiserAsync<OmdbMovieInfo>(appraisal);
                }

                case _steamAppraiserParameterName:
                {
                    var appraisal = new Appraisers.Appraisals.Game.Steam.SteamCommonAppraisal();

                    return new Appraisers.AppraiserAsync<SteamGameInfo>(appraisal);
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
        /// Creates outputter (async) instance depend on parameter value (could be get from config).
        /// </summary>
        /// <param name="outputterElement">Element from XML config.</param>
        /// <returns>Fully initialized instance of outputter interface.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="outputterElement" /> isn't specified in config.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="outputterElement" /> is <c>null</c>.
        /// </exception>
        public IO.Output.IOutputterAsync CreateOutputter(XElement outputterElement)
        {
            outputterElement.ThrowIfNull(nameof(outputterElement));

            _logger.Info("Creating outputter.");

            switch (outputterElement.Name.LocalName)
            {
                case _localFileParameterName:
                {
                    return new IO.Output.File.LocalFileWriterAsync();
                }

                case _googleDriveParameterName:
                {
                    throw new NotImplementedException("Now GoogleDrive isn't supported.");
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
    }
}
