using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using OMDbApiNet;
using OMDbApiNet.Model;
using ThingAppraiser.Logging;
using ThingAppraiser.Data;
using ThingAppraiser.Communication;
using ThingAppraiser.Crawlers.Mappers;

namespace ThingAppraiser.Crawlers
{
    /// <summary>
    /// Provides async version of OMDB crawler.
    /// </summary>
    public class OmdbCrawlerAsync : CrawlerAsync
    {
        /// <summary>
        /// Logger instance for current class.
        /// </summary>
        private static readonly LoggerAbstraction _logger =
            LoggerAbstraction.CreateLoggerInstanceFor<OmdbCrawlerAsync>();

        /// <summary>
        /// Helper class to transform raw DTO objects to concrete object without extra data.
        /// </summary>
        private readonly IDataMapper<Item, OmdbMovieInfo> _dataMapper =
            new DataMapperOmdbMovie();

        /// <summary>
        /// Key to get access to OMDB service.
        /// </summary>
        private readonly string _apiKey;

        /// <summary>
        /// Third-party helper class to make a calls to OMDB API.
        /// </summary>
        private readonly AsyncOmdbClient _omdbClient;

        /// <inheritdoc />
        public override string Tag { get; } = "OmdbCrawlerAsync";

        /// <inheritdoc />
        public override Type TypeId { get; } = typeof(OmdbMovieInfo);


        /// <summary>
        /// Initializes instance according to parameter values.
        /// </summary>
        /// <param name="apiKey">Key to get access to OMDB service.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="apiKey" /> is <c>null</c>, presents empty strings or contains only 
        /// whitespaces.
        /// </exception>
        public OmdbCrawlerAsync(string apiKey)
        {
            _apiKey = apiKey.ThrowIfNullOrWhiteSpace(nameof(apiKey));

            _omdbClient = new AsyncOmdbClient(_apiKey);
        }

        /// <inheritdoc />
        public override async Task<bool> GetResponse(BufferBlock<string> entitiesQueue,
            BufferBlock<BasicInfo> responsesQueue, bool outputResults)
        {
            // Use HashSet to avoid duplicated data which can produce errors in further work.
            var searchResults = new HashSet<BasicInfo>();
            while (await entitiesQueue.OutputAvailableAsync())
            {
                string movie = await entitiesQueue.ReceiveAsync();

                Item response = await _omdbClient.GetItemByTitleAsync(movie);
                if (!response.Response.IsEqualWithInvariantCulture("True"))
                {
                    _logger.Warn($"{movie} wasn't processed.");
                    GlobalMessageHandler.OutputMessage($"{movie} wasn't processed.");
                    continue;
                }

                // Get first search result from response and ignore all the rest.
                if (outputResults)
                {
                    GlobalMessageHandler.OutputMessage($"Got {response.Title}");
                }

                OmdbMovieInfo extractedInfo = _dataMapper.Transform(response);
                if (searchResults.Add(extractedInfo))
                {
                    await responsesQueue.SendAsync(extractedInfo);
                }
            }
            return searchResults.Count != 0;
        }
    }
}
