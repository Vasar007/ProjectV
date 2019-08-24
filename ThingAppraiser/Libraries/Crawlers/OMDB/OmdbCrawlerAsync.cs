using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using ThingAppraiser.Logging;
using ThingAppraiser.Communication;
using ThingAppraiser.Models.Data;
using ThingAppraiser.OmdbService;

namespace ThingAppraiser.Crawlers.Omdb
{
    /// <summary>
    /// Provides async version of OMDb crawler.
    /// </summary>
    public sealed class OmdbCrawlerAsync : CrawlerAsync
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

        /// <inheritdoc />
        public override string Tag { get; } = nameof(OmdbCrawlerAsync);

        /// <inheritdoc />
        public override Type TypeId { get; } = typeof(OmdbMovieInfo);


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
        public override async Task<bool> GetResponse(BufferBlock<string> entitiesQueue,
            BufferBlock<BasicInfo> responsesQueue, bool outputResults)
        {
            // Use HashSet to avoid duplicated data which can produce errors in further work.
            var searchResults = new HashSet<BasicInfo>();
            while (await entitiesQueue.OutputAvailableAsync())
            {
                string movie = await entitiesQueue.ReceiveAsync();

                OmdbMovieInfo response = await _omdbClient.TryGetItemByTitleAsync(movie);

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
    }
}
