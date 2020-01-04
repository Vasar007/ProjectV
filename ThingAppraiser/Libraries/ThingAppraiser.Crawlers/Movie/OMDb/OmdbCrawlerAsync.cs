using System;
using System.Collections.Generic;
using Acolyte.Assertions;
using ThingAppraiser.Communication;
using ThingAppraiser.Logging;
using ThingAppraiser.Models.Data;
using ThingAppraiser.OmdbService;

namespace ThingAppraiser.Crawlers.Movie.Omdb
{
    /// <summary>
    /// Provides async version of OMDb crawler.
    /// </summary>
    public sealed class OmdbCrawlerAsync : ICrawlerAsync, ICrawlerBase, IDisposable, ITagable,
        ITypeId
    {
        /// <summary>
        /// Logger instance for current class.
        /// </summary>
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor<OmdbCrawlerAsync>();

        /// <summary>
        /// Adapter class to make a calls to OMDb API.
        /// </summary>
        private readonly IOmdbClient _omdbClient;

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
        public string Tag { get; } = nameof(OmdbCrawlerAsync);

        #endregion

        #region ITypeId Implementation

        /// <inheritdoc />
        public Type TypeId { get; } = typeof(OmdbMovieInfo);

        #endregion


        /// <summary>
        /// Initializes instance according to parameter values.
        /// </summary>
        /// <param name="apiKey">Key to get access to OMDb service.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="apiKey" /> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="apiKey" /> presents empty strings or contains only whitespaces.
        /// </exception>
        public OmdbCrawlerAsync(string apiKey)
        {
            apiKey.ThrowIfNullOrWhiteSpace(nameof(apiKey));

            _omdbClient = OmdbClientFactory.CreateClient(apiKey);
            _searchResults = new HashSet<BasicInfo>();
        }

        #region ICrawlerAsync Implementation

        /// <inheritdoc />
        public async IAsyncEnumerable<BasicInfo> GetResponse(string entityName, bool outputResults)
        {
            OmdbMovieInfo? response = await _omdbClient.TryGetItemByTitleAsync(entityName);

            if (response is null)
            {
                string message = $"{entityName} was not processed.";
                _logger.Warn(message);
                GlobalMessageHandler.OutputMessage(message);

                yield break;
            }

            // Get first search result from response and ignore all the rest.
            if (outputResults)
            {
                GlobalMessageHandler.OutputMessage($"Got {response.Title} from \"{Tag}\".");
            }

            if (_searchResults.Add(response))
            {
                yield return response;
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

            _omdbClient.Dispose();
        }

        #endregion
    }
}
