using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThingAppraiser.Communication;
using ThingAppraiser.Extensions;
using ThingAppraiser.Logging;
using ThingAppraiser.Models.Data;
using ThingAppraiser.Models.Internal;
using ThingAppraiser.TmdbService;
using ThingAppraiser.TmdbService.Models;

namespace ThingAppraiser.Crawlers.Movie.Tmdb
{
    /// <summary>
    /// Provides async version of TMDb crawler.
    /// </summary>
    public sealed class TmdbCrawlerAsync : ICrawlerAsync, ICrawlerBase, IDisposable, ITagable,
        ITypeId
    {
        /// <summary>
        /// Logger instance for current class.
        /// </summary>
        private static readonly ILogger _logger = LoggerFactory.CreateLoggerFor<TmdbCrawlerAsync>();

        /// <summary>
        /// Adapter class to make a calls to TMDb API.
        /// </summary>
        private readonly ITmdbClient _tmdbClient;

        /// <summary>
        /// Uses <see cref="HashSet{T}" /> to avoid duplicated data which can produce errors in
        /// further work.
        /// </summary>
        private readonly HashSet<BasicInfo> _searchResults;

        /// <summary>
        /// Boolean flag used to show that object has already been disposed.
        /// </summary>
        private bool _disposed;

        #region ITagable Implementation

        /// <inheritdoc />
        public string Tag { get; } = nameof(TmdbCrawlerAsync);

        #endregion

        #region ITypeId Implementation

        /// <inheritdoc />
        public Type TypeId { get; } = typeof(TmdbMovieInfo);

        #endregion


        /// <summary>
        /// Initializes instance according to parameter values.
        /// </summary>
        /// <param name="apiKey">Key to get access to TMDb service.</param>
        /// <param name="maxRetryCount">Maximum retry number to get response from TMDb.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="apiKey" /> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="apiKey" /> presents empty string or contains only whitespaces.
        /// </exception>
        public TmdbCrawlerAsync(string apiKey, int maxRetryCount)
        {
            apiKey.ThrowIfNullOrWhiteSpace(nameof(apiKey));

            _tmdbClient = TmdbClientFactory.CreateClient(apiKey, maxRetryCount);
            _searchResults = new HashSet<BasicInfo>();
        }

        #region ICrawlerAsync Implemenation

        /// <inheritdoc />
        public async IAsyncEnumerable<BasicInfo> GetResponse(string entityName, bool outputResults)
        {
            TmdbServiceConfiguration.SetServiceConfigurationOnce(
                await GetServiceConfiguration(outputResults)
            );

            TmdbSearchContainer? response = await _tmdbClient.TrySearchMovieAsync(entityName);

            if (response is null || response.Results.IsNullOrEmpty())
            {
                string message = $"{entityName} was not processed.";
                _logger.Warn(message);
                GlobalMessageHandler.OutputMessage(message);

                yield break;
            }

            // Get first search result from response and ignore all the rest.
            TmdbMovieInfo searchResult = response.Results.First();
            if (outputResults)
            {
                GlobalMessageHandler.OutputMessage($"Got {searchResult.Title} from \"{Tag}\".");
            }

            if (_searchResults.Add(searchResult))
            {
                yield return searchResult;
            }

            yield break;
        }

        #endregion

        #region IDisposable Implementation

        /// <summary>
        /// Releases resources of TMDb client.
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            _tmdbClient.Dispose();
        }

        #endregion

        /// <summary>
        /// Gets service configuration.
        /// </summary>
        /// <param name="outputResults">Flag to define need to output.</param>
        /// <returns>Transformed configuration of the service.</returns>
        private async Task<TmdbServiceConfigurationInfo> GetServiceConfiguration(
            bool outputResults)
        {
            TmdbServiceConfigurationInfo config = await _tmdbClient.GetConfigAsync();

            if (outputResults)
            {
                GlobalMessageHandler.OutputMessage("Got TMDb config.");
            }

            return config;
        }
    }
}
