using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
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
    public class TmdbCrawlerRx : CrawlerRx
    {
        private static readonly LoggerAbstraction _logger =
            LoggerAbstraction.CreateLoggerInstanceFor<TmdbCrawlerRx>();

        private readonly IDataMapper<SearchMovie, TmdbMovieInfo> _dataMapper =
            new DataMapperTmdbMovie();

        private readonly IDataMapper<TMDbConfig, TmdbServiceConfigurationInfo> _configMapper =
            new DataMapperTmdbConfig();

        private readonly ConcurrentDictionary<BasicInfo, byte> _searchResults;

        private readonly string _apiKey;

        private readonly TMDbClient _tmdbClient;

        /// <inheritdoc />
        public override string Tag { get; } = "TmdbCrawlerRx";

        /// <inheritdoc />
        public override Type TypeId { get; } = typeof(TmdbMovieInfo);


        public TmdbCrawlerRx(string apiKey, int maxRetryCount)
        {
            _apiKey = apiKey.ThrowIfNullOrWhiteSpace(nameof(apiKey));

            _tmdbClient = new TMDbClient(_apiKey)
            {
                MaxRetryCount = maxRetryCount
            };
            _searchResults = new ConcurrentDictionary<BasicInfo, byte>();
        }

        #region CrawlerSync Overridden Methods

        public override async Task<BasicInfo> FindResponse(string entity, bool outputResults)
        {
            if (!TmdbServiceConfiguration.HasValue())
            {
                TmdbServiceConfiguration.SetServiceConfigurationIfNeed(
                    await GetServiceConfiguration(outputResults)
                );
            }

            SearchContainer<SearchMovie> response = await _tmdbClient.SearchMovieAsync(entity);
            if (response.Results.IsNullOrEmpty())
            {
                _logger.Warn($"{entity} wasn't processed.");
                GlobalMessageHandler.OutputMessage($"{entity} wasn't processed.");
                return null;
            }

            // Get first search result from response and ignore all the rest.
            SearchMovie searchResult = response.Results.First();
            if (outputResults)
            {
                GlobalMessageHandler.OutputMessage($"Got {searchResult.Title} from {Tag}");
            }

            TmdbMovieInfo extractedInfo = _dataMapper.Transform(searchResult);

            if (_searchResults.TryAdd(extractedInfo, default))
            {
                return extractedInfo;
            }

            GlobalMessageHandler.OutputMessage($"{entity} has been processed already.");
            return null;
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