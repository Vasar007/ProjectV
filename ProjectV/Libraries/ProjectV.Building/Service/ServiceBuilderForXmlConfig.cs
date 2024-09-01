#pragma warning disable format // dotnet format fails indentation for switch :(

using System;
using System.Xml.Linq;
using Acolyte.Assertions;
using Acolyte.Xml;
using ProjectV.Logging;
using ProjectV.Models.Data;

namespace ProjectV.Building.Service
{
    /// <summary>
    /// Provides methods to create service classes instances with parameters from XML 
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
        /// Creates service builder to interact with XML config.
        /// </summary>
        public ServiceBuilderForXmlConfig()
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

            switch (handlerElement!.Value)
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
        /// Creates inputter instance depend on parameter value (could be get from config).
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

            _logger.Info("Creating intputter.");

            switch (inputterElement.Name.LocalName)
            {
                case _localFileParameterName:
                {
                    string fileReaderName = XDocumentParser.GetAttributeValue(
                        inputterElement, _fileReaderParameterName + _localFileParameterName
                    );

                    var fileReader = CreateFileReader(fileReaderName);

                    return new IO.Input.File.LocalFileReader(fileReader);
                }

                case _googleDriveParameterName:
                {
                    string fileReaderName = XDocumentParser.GetAttributeValue(
                        inputterElement, _fileReaderParameterName + _googleDriveParameterName
                    );

                    var fileReader = CreateFileReader(fileReaderName);

                    return new IO.Input.GoogleDrive.GoogleDriveReader(DriveService, fileReader);
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
        /// Creates crawler instance depend on parameter value (could be get from config).
        /// </summary>
        /// <param name="crawlerElement">Element from XML config.</param>
        /// <returns>Fully initialized instance of crawler class.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="crawlerElement" /> isn't specified in config.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="crawlerElement" /> is <c>null</c>.
        /// </exception>
        public Crawlers.ICrawler CreateCrawler(XElement crawlerElement)
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

                    return new Crawlers.Movie.Tmdb.TmdbCrawler(apiKey, maxRetryCount);
                }

                case _omdbCrawlerParameterName:
                {
                    string apiKey = XDocumentParser.GetAttributeValue(
                        crawlerElement, _omdbApiKeyParameterName
                    );

                    return new Crawlers.Movie.Omdb.OmdbCrawler(apiKey);
                }

                case _steamCrawlerParameterName:
                {
                    string apiKey = XDocumentParser.GetAttributeValue(
                        crawlerElement, _steamApiKeyParameterName
                    );

                    return new Crawlers.Game.Steam.SteamCrawler(apiKey);
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
        /// Creates appraiser instance depend on parameter value (could be get from config).
        /// </summary>
        /// <param name="appraiserElement">Element from XML config.</param>
        /// <returns>Fully initialized instance of appraiser class.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="appraiserElement" /> isn't specified in config.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="appraiserElement" /> is <c>null</c>.
        /// </exception>
        public Appraisers.IAppraiser CreateAppraiser(XElement appraiserElement)
        {
            appraiserElement.ThrowIfNull(nameof(appraiserElement));

            _logger.Info("Creating appraiser.");

            switch (appraiserElement.Name.LocalName)
            {
                case _appraiserTmdbParameterName:
                {
                    var appraisal = new Appraisers.Appraisals.Movie.Tmdb.TmdbCommonAppraisal();

                    return new Appraisers.Appraiser<TmdbMovieInfo>(appraisal);
                }

                case _appraiserOmdbParameterName:
                {
                    var appraisal = new Appraisers.Appraisals.Movie.Omdb.OmdbCommonAppraisal();

                    return new Appraisers.Appraiser<OmdbMovieInfo>(appraisal);
                }

                case _steamAppraiserParameterName:
                {
                    var appraisal = new Appraisers.Appraisals.Game.Steam.SteamCommonAppraisal();

                    return new Appraisers.Appraiser<SteamGameInfo>(appraisal);
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
        /// Creates outputter instance depend on parameter value (could be get from config).
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
                    return new IO.Output.File.LocalFileWriterAsync();
                }

                case _googleDriveParameterName:
                {
                    return new IO.Output.GoogleDrive.GoogleDriveWriter(DriveService);
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
