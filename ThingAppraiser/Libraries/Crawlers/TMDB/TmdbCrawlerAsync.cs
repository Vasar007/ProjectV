using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using ThingAppraiser.Communication;
using ThingAppraiser.Logging;
using ThingAppraiser.Models.Data;
using ThingAppraiser.Models.Internal;
using ThingAppraiser.TmdbService;
using ThingAppraiser.TmdbService.Models;

namespace ThingAppraiser.Crawlers.Tmdb
{
    public sealed class TmdbCrawlerAsync : CrawlerAsync
    {
        private static readonly ILogger _logger = LoggerFactory.CreateLoggerFor<TmdbCrawlerAsync>();

        private readonly string _apiKey;

        private readonly ITmdbClient _tmdbClient;

        /// <inheritdoc />
        public override string Tag { get; } = nameof(TmdbCrawlerAsync);

        /// <inheritdoc />
        public override Type TypeId { get; } = typeof(TmdbMovieInfo);


        public TmdbCrawlerAsync(string apiKey, int maxRetryCount)
        {
            _apiKey = apiKey.ThrowIfNullOrWhiteSpace(nameof(apiKey));

            _tmdbClient = TmdbClientFactory.CreateClient(_apiKey, maxRetryCount);
        }

        #region CrawlerAsync Overridden Methods

        public override async Task<bool> GetResponse(BufferBlock<string> entitiesQueue,
            BufferBlock<BasicInfo> responsesQueue, bool outputResults)
        {
            TmdbServiceConfiguration.SetServiceConfigurationIfNeed(
                await GetServiceConfiguration(outputResults)
            );

            // Use HashSet to avoid duplicated data which can produce errors in further work.
            var searchResults = new HashSet<BasicInfo>();
            while (await entitiesQueue.OutputAvailableAsync())
            {
                string movie = await entitiesQueue.ReceiveAsync();

                TmdbSearchContainer response = await _tmdbClient.SearchMovieAsync(movie);
                if (response.Results.IsNullOrEmpty())
                {
                    _logger.Warn($"{movie} wasn't processed.");
                    GlobalMessageHandler.OutputMessage($"{movie} wasn't processed.");
                    continue;
                }

                // Get first search result from response and ignore all the rest.
                TmdbMovieInfo searchResult = response.Results.First();
                if (outputResults)
                {
                    GlobalMessageHandler.OutputMessage($"Got {searchResult.Title} from \"{Tag}\".");
                }

                if (searchResults.Add(searchResult))
                {
                    await responsesQueue.SendAsync(searchResult);
                }
            }
            return searchResults.Count != 0;
        }

        #endregion

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
