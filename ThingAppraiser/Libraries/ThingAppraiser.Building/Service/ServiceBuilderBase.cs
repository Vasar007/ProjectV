using System;
using System.IO;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using ThingAppraiser.Extensions;
using ThingAppraiser.Logging;

namespace ThingAppraiser.Building.Service
{
    /// <summary>
    /// Contains identifiers to create proper type of components.
    /// </summary>
    public abstract class ServiceBuilderBase
    {
        /// <summary>
        /// Logger instance for current class.
        /// </summary>
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor<ServiceBuilderBase>();

        /// <summary>
        /// Attribute value for console message handler.
        /// </summary>
        protected const string _consoleMessageHandlerParameterName = "ConsoleMessageHandler";

        /// <summary>
        /// Attribute value for local file handlers.
        /// </summary>
        protected const string _localFileParameterName = "LocalFile";

        /// <summary>
        /// Attribute value for Google Drive handlers.
        /// </summary>
        protected const string _googleDriveParameterName = "GoogleDrive";

        /// <summary>
        /// Attribute value for TMDb crawler.
        /// </summary>
        protected const string _tmdbCrawlerParameterName = "TmdbCrawler";

        /// <summary>
        /// Attribute value for OMDb crawler.
        /// </summary>
        protected const string _omdbCrawlerParameterName = "OmdbCrawler";

        /// <summary>
        /// Attribute value for Steam crawler.
        /// </summary>
        protected const string _steamCrawlerParameterName = "SteamCrawler";

        /// <summary>
        /// Attribute value for TMDb appraiser.
        /// </summary>
        protected const string _appraiserTmdbParameterName = "TmdbAppraiser";

        /// <summary>
        /// Attribute value for TMDb appraiser which based on Fuzzy Logic Toolbox.
        /// </summary>
        protected const string _fuzzyAppraiserTmdbParameterName = "FuzzyTmdbAppraiser";

        /// <summary>
        /// Attribute value for OMDb appraiser.
        /// </summary>
        protected const string _appraiserOmdbParameterName = "OmdbAppraiser";

        /// <summary>
        /// Attribute value for Steam appraiser.
        /// </summary>
        protected const string _steamAppraiserParameterName = "SteamAppraiser";

        /// <summary>
        /// Attribute value for basic info repository.
        /// </summary>
        protected const string _basicInfoRepositoryParameterName = "BasicInfoRepository";

        /// <summary>
        /// Attribute value for basic info repository.
        /// </summary>
        protected const string _tmdbMovieRepositoryParameterName = "TmdbMovieRepository";

        /// <summary>
        /// Attribute name for message handler.
        /// </summary>
        protected static readonly string _messageHandlerTypeParameterName = "MessageHandlerType";

        /// <summary>
        /// Attribute name for flag which represents changing default encoding of message handler.
        /// </summary>
        protected static readonly string _setUnicodeParameterName =
            "ConsoleMessageHandlerSetUnicode";

        /// <summary>
        /// Attribute name for inputters to define instance of file reader.
        /// </summary>
        protected static readonly string _fileReaderParameterName = "FileReader";

        /// <summary>
        /// Attribute name for API key of TMDb crawler.
        /// </summary>
        protected static readonly string _tmdbApiKeyParameterName = "TmdbApiKey";

        /// <summary>
        /// Attribute name for number of maximum attemts to retry for TMDb crawler.
        /// </summary>
        protected static readonly string _tmdbMaxRetryCountParameterName = "TmdbMaxRetryCount";

        /// <summary>
        /// Attribute name for API key of OMDb crawler.
        /// </summary>
        protected static readonly string _omdbApiKeyParameterName = "OmdbApiKey";

        /// <summary>
        /// Attribute name for API key of TMDb crawler.
        /// </summary>
        protected static readonly string _steamApiKeyParameterName = "SteamApiKey";

        /// <summary>
        /// Provides thread-safe service to interact with Google Drive.
        /// </summary>
        protected static readonly DriveService _driveService = CreateDriveService();

        /// <summary>
        /// Attribute value for simple file reader.
        /// </summary>
        private const string _simpleFileReaderParameterName = "Simple";

        /// <summary>
        /// Attribute value for filter file reader.
        /// </summary>
        private const string _filterFileReaderParameterName = "Filter";


        /// <summary>
        /// Initializes base class.
        /// </summary>
        protected ServiceBuilderBase()
        {
        }

        /// <summary>
        /// Creates file reader (sequential) instance depend on parameter value (could be read from 
        /// config file or XML document).
        /// </summary>
        /// <param name="fileReaderName">
        /// Name of the file reader (sequential) class to create.
        /// </param>
        /// <returns>Fully initialized instance of file reader (sequential) class.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="fileReaderName" /> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="fileReaderName" /> isn't specified in method.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="fileReaderName" /> presents empty string.
        /// </exception>
        protected IO.Input.File.IFileReader CreateFileReader(string fileReaderName)
        {
            fileReaderName.ThrowIfNullOrEmpty(nameof(fileReaderName));

            _logger.Info("Creating file reader.");

            switch (fileReaderName)
            {
                case _simpleFileReaderParameterName:
                {
                    return new IO.Input.File.SimpleFileReader();
                }

                case _filterFileReaderParameterName:
                {
                    return new IO.Input.File.FilterFileReader();
                }

                default:
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(fileReaderName), fileReaderName,
                        "Couldn't recognize file reader type."
                    );
                }
            }
        }

        /// <summary>
        /// Creates file reader (async) instance depend on parameter value (could be read from 
        /// config file or XML document).
        /// </summary>
        /// <param name="fileReaderName">Name of the file reader (async) class to create.</param>
        /// <returns>Fully initialized instance of file reader (async) class.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="fileReaderName" /> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="fileReaderName" /> isn't specified in method.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="fileReaderName" /> presents empty string.
        /// </exception>
        protected IO.Input.File.IFileReaderAsync CreateFileReaderAsync(string fileReaderName)
        {
            fileReaderName.ThrowIfNullOrEmpty(nameof(fileReaderName));

            _logger.Info("Creating async file reader.");

            switch (fileReaderName)
            {
                case _simpleFileReaderParameterName:
                {
                    return new IO.Input.File.SimpleFileReaderAsync();
                }

                case _filterFileReaderParameterName:
                {
                    throw new NotImplementedException("Now FilterFileReaderAsync isn't supported.");
                }

                default:
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(fileReaderName), fileReaderName,
                        "Couldn't recognize file reader type."
                    );
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
            _logger.Info("Creating Google Drive service.");

            UserCredential credential;
            using (var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                const string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    IO.GoogleDriveWorker.Scopes,
                    IO.GoogleDriveWorker.ApplicationName,
                    CancellationToken.None,
                    new FileDataStore(credPath, true)
                ).Result;

                _logger.Info($"Credential file saved to: \"{credPath}\".");
            }

            // Create Drive API service.
            return new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = IO.GoogleDriveWorker.ApplicationName
            });
        }
    }
}
