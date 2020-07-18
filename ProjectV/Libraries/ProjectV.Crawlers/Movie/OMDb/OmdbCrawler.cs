using System;
using System.Collections.Generic;
using System.Linq;
using Acolyte.Assertions;
using ProjectV.Communication;
using ProjectV.Logging;
using ProjectV.Models.Data;
using ProjectV.OmdbService;

namespace ProjectV.Crawlers.Movie.Omdb
{
    /// <summary>
    /// Concrete crawler for Open Movie Database service.
    /// </summary>
    public sealed class OmdbCrawler : ICrawler, ICrawlerBase, IDisposable, ITagable, ITypeId
    {
        /// <summary>
        /// Logger instance for current class.
        /// </summary>
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor<OmdbCrawler>();

        /// <summary>
        /// Adapter class to make a calls to OMDb API.
        /// </summary>
        private readonly IOmdbClient _omdbClient;

        /// <summary>
        /// Boolean flag used to show that object has already been disposed.
        /// </summary>
        private bool _disposed;

        #region ITagable Implementation

        /// <inheritdoc />
        public string Tag { get; } = nameof(OmdbCrawler);

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
        public OmdbCrawler(string apiKey)
        {
            apiKey.ThrowIfNullOrWhiteSpace(nameof(apiKey));

            _omdbClient = OmdbClientFactory.CreateClient(apiKey);
        }

        #region ICrawler Implementation

        /// <inheritdoc />
        public IReadOnlyList<BasicInfo> GetResponse(IReadOnlyList<string> entities,
            bool outputResults)
        {
            // Use HashSet to avoid duplicated data which can produce errors in further work.
            var searchResults = new HashSet<BasicInfo>();
            foreach (string movie in entities)
            {
                OmdbMovieInfo? response = _omdbClient.TryGetItemByTitleAsync(movie).Result;

                if (response is null)
                {
                    string message = $"{movie} was not processed.";
                    _logger.Warn(message);
                    GlobalMessageHandler.OutputMessage(message);

                    continue;
                }

                // Get first search result from response and ignore all the rest.
                if (outputResults)
                {
                    GlobalMessageHandler.OutputMessage($"Got {response.Title} from \"{Tag}\".");
                }

                searchResults.Add(response);
            }
            return searchResults.ToList();
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
