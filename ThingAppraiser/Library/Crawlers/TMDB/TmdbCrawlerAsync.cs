using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using TMDbLib.Client;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Search;
using ThingAppraiser.Communication;
using ThingAppraiser.Crawlers.Mappers;
using ThingAppraiser.Data;
using ThingAppraiser.Data.Crawlers;
using ThingAppraiser.Logging;

namespace ThingAppraiser.Crawlers
{
    public class TmdbCrawlerAsync : CrawlerAsync
    {
        private static readonly LoggerAbstraction _logger =
            LoggerAbstraction.CreateLoggerInstanceFor<TmdbCrawlerAsync>();

        private readonly IDataMapper<SearchMovie, TmdbMovieInfo> _dataMapper =
            new DataMapperTmdbMovie();

        private readonly IDataMapper<TMDbConfig, TmdbServiceConfigurationInfo> _configMapper =
            new DataMapperTmdbConfig();

        private readonly string _apiKey;

        private readonly TMDbClient _tmdbClient;

        /// <inheritdoc />
        public override string Tag { get; } = "TmdbCrawlerAsync";

        /// <inheritdoc />
        public override Type TypeId { get; } = typeof(TmdbMovieInfo);


        public TmdbCrawlerAsync(string apiKey, int maxRetryCount)
        {
            _apiKey = apiKey.ThrowIfNullOrWhiteSpace(nameof(apiKey));

            _tmdbClient = new TMDbClient(_apiKey)
            {
                MaxRetryCount = maxRetryCount
            };
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

                SearchContainer<SearchMovie> response = await _tmdbClient.SearchMovieAsync(movie);
                if (response.Results.IsNullOrEmpty())
                {
                    _logger.Warn($"{movie} wasn't processed.");
                    GlobalMessageHandler.OutputMessage($"{movie} wasn't processed.");
                    continue;
                }

                // Get first search result from response and ignore all the rest.
                SearchMovie searchResult = response.Results.First();
                if (outputResults)
                {
                    GlobalMessageHandler.OutputMessage($"Got {searchResult.Title} from {Tag}");
                }

                TmdbMovieInfo extractedInfo = _dataMapper.Transform(searchResult);
                if (searchResults.Add(extractedInfo))
                {
                    await responsesQueue.SendAsync(extractedInfo);
                }
            }
            return searchResults.Count != 0;
        }

        #endregion

        private async Task<TmdbServiceConfigurationInfo> GetServiceConfiguration(
            bool outputResults)
        {
            var config = await _tmdbClient.GetConfigAsync();

            if (_tmdbClient.Config.Images is null)
            {
                _logger.Warn("Image configuration cannot be obtained.");
                GlobalMessageHandler.OutputMessage("Image configuration cannot be obtained.");
            }

            if (outputResults)
            {
                GlobalMessageHandler.OutputMessage("Got TMDb config.");
            }

            return _configMapper.Transform(config);
        }
    }
}
