using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TMDbLib.Client;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Search;
using ThingAppraiser.Communication;
using ThingAppraiser.Extensions;
using ThingAppraiser.Logging;
using ThingAppraiser.Models.Exceptions;
using ThingAppraiser.Models.Internal;
using ThingAppraiser.TmdbService.Mappers;
using ThingAppraiser.TmdbService.Models;

namespace ThingAppraiser.TmdbService
{
    internal sealed class TmdbClient : ITmdbClient, IDisposable
    {
        /// <summary>
        /// Logger instance for current class.
        /// </summary>
        private static readonly ILogger _logger = LoggerFactory.CreateLoggerFor<TmdbClient>();

        /// <summary>
        /// Helper class to transform raw DTO objects to concrete object without extra data.
        /// </summary>
        private readonly IDataMapper<SearchContainer<SearchMovie>, TmdbSearchContainer> _dataMapper
            = new DataMapperTmdbContainer();

        /// <summary>
        /// Helper class to transform raw DTO config to concrete interanl object without extra data.
        /// </summary>
        private readonly IDataMapper<TMDbConfig, TmdbServiceConfigurationInfo> _configMapper =
            new DataMapperTmdbConfig();

        /// <summary>
        /// Third-party helper class to make a calls to TMDb API.
        /// </summary>
        private readonly TMDbClient _tmdbClient;

        private bool _disposed;

        /// <inheritdoc />
        public int MaxRetryCount
        {
            get => _tmdbClient.MaxRetryCount;
            set => _tmdbClient.MaxRetryCount = value;
        }

        /// <inheritdoc />
        public bool ThrowApiExceptions
        {
            get => _tmdbClient.ThrowApiExceptions;
            set => _tmdbClient.ThrowApiExceptions = value;
        }

        public bool HasConfig => _tmdbClient.HasConfig;

        /// <inheritdoc />
        public string DefaultLanguage
        {
            get => _tmdbClient.DefaultLanguage;
            set => _tmdbClient.DefaultLanguage = value.ThrowIfNull(nameof(value));
        }

        /// <inheritdoc />
        public string DefaultCountry
        {
            get => _tmdbClient.DefaultCountry;
            set => _tmdbClient.DefaultCountry = value.ThrowIfNull(nameof(value));
        }
        
        public TmdbServiceConfigurationInfo Config => _configMapper.Transform(_tmdbClient.Config);

        /// <inheritdoc />
        public string ApiKey => _tmdbClient.ApiKey;

        /// <inheritdoc />
        public IWebProxy WebProxy => _tmdbClient.WebProxy;


        public TmdbClient(string apiKey, bool useSsl = false, string baseUrl = "api.themoviedb.org",
            JsonSerializer? serializer = null, IWebProxy? proxy = null)
        {
            apiKey.ThrowIfNullOrWhiteSpace(nameof(apiKey));
            baseUrl.ThrowIfNullOrWhiteSpace(nameof(apiKey));

            _tmdbClient = new TMDbClient(apiKey, useSsl, baseUrl, serializer, proxy);
        }

        public async Task<TmdbServiceConfigurationInfo> GetConfigAsync()
        {
            _logger.Info("Getting TMBD config.");

            TMDbConfig config = await _tmdbClient.GetConfigAsync();

            if (config?.Images is null)
            {
                string message = "TMDb image configuration cannot be obtained.";
                GlobalMessageHandler.OutputMessage(message);

                throw new CannotGetTmdbConfigException(message);
            }

            return _configMapper.Transform(config);
        }

        public async Task<TmdbSearchContainer?> TrySearchMovieAsync(string query, int page = 0,
            bool includeAdult = false, int year = 0, CancellationToken cancellationToken = default)
        {
            query.ThrowIfNullOrWhiteSpace(nameof(query));

            _logger.Info($"Searching TMDb movie by query: \"{query}\" [page: {page.ToString()}, " +
                         $"includeAdult: {includeAdult.ToString()}, year: {year.ToString()}].");

            try
            {
                SearchContainer<SearchMovie> response = await _tmdbClient.SearchMovieAsync(
                    query, page, includeAdult, year, cancellationToken
                );

                return _dataMapper.Transform(response);
            }
            catch (Exception ex)
            {
                _logger.Warn(ex, $"Exception occurred during processing response for \"{query}\".");

                return null;
            }
        }

        #region IDisposable Implementation

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            _tmdbClient.Dispose();
        }

        #endregion
    }
}
