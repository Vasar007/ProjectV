using System;
using ThingAppraiser.Logging;

namespace ThingAppraiser.Core.Building
{
    /// <summary>
    /// Provides creating methods for service classes with parameters from AppConfig.xml file.
    /// </summary>
    public sealed class ServiceBuilderForAppConfig : ServiceBuilderBase
    {
        /// <summary>
        /// Logger instance for current class.
        /// </summary>
        private static readonly LoggerAbstraction _logger =
            LoggerAbstraction.CreateLoggerInstanceFor<ServiceBuilderForAppConfig>();


        /// <summary>
        /// Creates service builder to interact with AppConfig.xml file.
        /// </summary>
        public ServiceBuilderForAppConfig()
        {
        }

        /// <summary>
        /// Creates message handler instance depend on parameter value (could be read from config 
        /// file).
        /// </summary>
        /// <param name="messageHandlerName">Name of the message handler class to create.</param>
        /// <returns>Fully initialized instance of message handler class.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="messageHandlerName" /> isn't specified in method.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="messageHandlerName" /> is <c>null</c> or presents empty string.
        /// </exception>
        public Communication.IMessageHandler CreateMessageHandler(string messageHandlerName)
        {
            messageHandlerName.ThrowIfNullOrEmpty(nameof(messageHandlerName));

            switch (messageHandlerName)
            {
                case _consoleMessageHandlerParameterName:
                {
                    var setUnicode = ConfigParser.GetValueByParameterKey<bool>(
                        _setUnicodeParameterName
                    );
                    return new Communication.ConsoleMessageHandler(setUnicode);
                }

                default:
                {
                    var ex = new ArgumentOutOfRangeException(
                        nameof(messageHandlerName), messageHandlerName,
                        "Couldn't recognize message handler type."
                    );
                    _logger.Error(ex, $"Passed incorrect data to method: {messageHandlerName}");
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
        /// <paramref name="inputterName" /> isn't specified in method.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="inputterName" /> is <c>null</c> or presents empty string.
        /// </exception>
        public IO.Input.IInputter CreateInputter(string inputterName)
        {
            inputterName.ThrowIfNullOrEmpty(nameof(inputterName));

            switch (inputterName)
            {
                case _localFileParameterName:
                {
                    string fileReaderName = ConfigParser.GetValueByParameterKey(
                        _fileReaderParameterName + _localFileParameterName
                    );

                    IO.Input.IFileReader fileReader = CreateFileReader(fileReaderName);

                    return new IO.Input.LocalFileReader(fileReader);
                }

                case _googleDriveParameterName:
                {
                    string fileReaderName = ConfigParser.GetValueByParameterKey(
                        _fileReaderParameterName + _googleDriveParameterName
                    );

                    IO.Input.IFileReader fileReader = CreateFileReader(fileReaderName);

                    return new IO.Input.GoogleDriveReader(_driveService, fileReader);
                }

                default:
                {
                    var ex = new ArgumentOutOfRangeException(nameof(inputterName), inputterName,
                                                             "Couldn't recognize input type.");
                    _logger.Error(ex, $"Passed incorrect data to method: {inputterName}");
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
        /// <paramref name="crawlerName" /> isn't specified in method.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="crawlerName" /> is <c>null</c> or presents empty string.
        /// </exception>
        public Crawlers.Crawler CreateCrawler(string crawlerName)
        {
            crawlerName.ThrowIfNullOrEmpty(nameof(crawlerName));

            switch (crawlerName)
            {
                case _tmdbCrawlerParameterName:
                {
                    string apiKey = ConfigParser.GetValueByParameterKey(
                        _tmdbApiKeyParameterName
                    );
                    var maxRetryCount = ConfigParser.GetValueByParameterKey<int>(
                        _tmdbMaxRetryCountParameterName
                    );

                    return new Crawlers.TmdbCrawler(apiKey, maxRetryCount);
                }

                case _steamCrawlerParameterName:
                {
                    string apiKey = ConfigParser.GetValueByParameterKey(
                        _steamApiKeyParameterName
                    );

                    return new Crawlers.SteamCrawler(apiKey);
                }

                default:
                { 
                    var ex = new ArgumentOutOfRangeException(nameof(crawlerName), crawlerName,
                                                             "Couldn't recognize crawler type.");
                    _logger.Error(ex, $"Passed incorrect data to method: {crawlerName}");
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
        /// <paramref name="appraiserName" /> isn't specified in method.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="appraiserName" /> is <c>null</c> or presents empty string.
        /// </exception>
        public Appraisers.Appraiser CreateAppraiser(string appraiserName)
        {
            appraiserName.ThrowIfNullOrEmpty(appraiserName);

            switch (appraiserName)
            {
                case _appraiserTmdbParameterName:
                {
                    return new Appraisers.TmdbAppraiser();
                }

                case _fuzzyAppraiserTmdbParameterName:
                {
                    return new Appraisers.FuzzyTmdbAppraiser();
                }

                default:
                {
                    var ex = new ArgumentOutOfRangeException(nameof(appraiserName), appraiserName,
                                                             "Couldn't recognize appraiser.");
                    _logger.Error(ex, $"Passed incorrect data to method: {appraiserName}");
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
        /// <paramref name="outputterName" /> isn't specified in method.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="outputterName" /> is <c>null</c> or presents empty string.
        /// </exception>
        public IO.Output.IOutputter CreateOutputter(string outputterName)
        {
            outputterName.ThrowIfNullOrEmpty(nameof(outputterName));

            switch (outputterName)
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
                    var ex = new ArgumentOutOfRangeException(nameof(outputterName), outputterName,
                                                             "Couldn't recognize output type.");
                    _logger.Error(ex, $"Passed incorrect data to method: {outputterName}");
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Creates repository instance depend on parameter value (could be read from config file).
        /// </summary>
        /// <param name="repositoryName">Name of the repository.</param>
        /// <returns>Fully initialized instance of repository interface.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="repositoryName" /> isn't specified in method.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="repositoryName" /> is <c>null</c> or presents empty string.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="storageSettings" /> is <c>null</c>.
        /// </exception>
        public DAL.Repositories.IDataRepository CreateRepository(string repositoryName,
            DAL.DataStorageSettings storageSettings)
        {
            repositoryName.ThrowIfNullOrEmpty(nameof(repositoryName));

            switch (repositoryName)
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
                    var ex = new ArgumentOutOfRangeException(nameof(repositoryName), repositoryName,
                                                             "Couldn't recognize repository type.");
                    _logger.Error(ex, $"Passed incorrect data to method: {repositoryName}");
                    throw ex;
                }
            }
        }
    }
}
