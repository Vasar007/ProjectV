using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using ThingAppraiser.Communication;
using ThingAppraiser.Extensions;
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
        }

        #region CrawlerAsync Overridden Methods

        /// <inheritdoc />
        public async Task<bool> GetResponse(ISourceBlock<string> entitiesQueue,
            ITargetBlock<BasicInfo> responsesQueue, bool outputResults)
        {
            // Use HashSet to avoid duplicated data which can produce errors in further work.
            var searchResults = new HashSet<BasicInfo>();
            while (await entitiesQueue.OutputAvailableAsync())
            {
                string movie = await entitiesQueue.ReceiveAsync();

                OmdbMovieInfo? response = await _omdbClient.TryGetItemByTitleAsync(movie);

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

                if (searchResults.Add(response))
                {
                    await responsesQueue.SendAsync(response);
                }
            }
            return searchResults.Count != 0;
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
