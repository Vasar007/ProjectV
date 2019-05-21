using System;
using System.Collections.Generic;
using System.Linq;
using TMDbLib.Client;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Search;
using ThingAppraiser.Logging;
using ThingAppraiser.Data;
using ThingAppraiser.Communication;
using ThingAppraiser.Crawlers.Mappers;
using ThingAppraiser.Data.Crawlers;

namespace ThingAppraiser.Crawlers
{
    /// <summary>
    /// Concrete crawler for The Movie Database service.
    /// </summary>
    public class TmdbCrawler : Crawler
    {
        /// <summary>
        /// Logger instance for current class.
        /// </summary>
        private static readonly LoggerAbstraction _logger =
            LoggerAbstraction.CreateLoggerInstanceFor<TmdbCrawler>();

        /// <summary>
        /// Helper class to transform raw DTO objects to concrete object without extra data.
        /// </summary>
        private readonly IDataMapper<SearchMovie, TmdbMovieInfo> _dataMapper =
            new DataMapperTmdbMovie();

        /// <summary>
        /// Helper class to transform raw DTO config to concrete interanl object without extra data.
        /// </summary>
        private readonly IDataMapper<TMDbConfig, TmdbServiceConfigurationInfo> _configMapper =
            new DataMapperTmdbConfig();

        /// <summary>
        /// Key to get access to TMDb service.
        /// </summary>
        private readonly string _apiKey;

        /// <summary>
        /// Third-party helper class to make a calls to TMDb API.
        /// </summary>
        private readonly TMDbClient _tmdbClient;

        /// <inheritdoc />
        public override string Tag { get; } = "TmdbCrawler";

        /// <inheritdoc />
        public override Type TypeId { get; } = typeof(TmdbMovieInfo);


        /// <summary>
        /// Initializes instance according to parameter values.
        /// </summary>
        /// <param name="apiKey">Key to get access to TMDb service.</param>
        /// <param name="maxRetryCount">Maximum retry number to get response from TMDb.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="apiKey" /> is <c>null</c>, presents empty string or contains only 
        /// whitespaces.
        /// </exception>
        public TmdbCrawler(string apiKey, int maxRetryCount)
        {
            _apiKey = apiKey.ThrowIfNullOrWhiteSpace(nameof(apiKey));

            _tmdbClient = new TMDbClient(_apiKey)
            {
                MaxRetryCount = maxRetryCount
            };
        }

        #region Crawler Overridden Methods

        /// <inheritdoc />
        public override List<BasicInfo> GetResponse(List<string> entities, bool outputResults)
        {
            if (!TmdbServiceConfiguration.HasValue())
            {
                TmdbServiceConfiguration.SetServiceConfigurationIfNeed(
                    GetServiceConfiguration(outputResults)
                );
            }

            // Use HashSet to avoid duplicated data which can produce errors in further work.
            var searchResults = new HashSet<BasicInfo>();
            foreach (string movie in entities)
            {
                SearchContainer<SearchMovie> response = _tmdbClient.SearchMovieAsync(movie).Result;
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
                searchResults.Add(extractedInfo);
            }
            return searchResults.ToList();
        }

        #endregion

        /// <summary>
        /// Gets service configuration.
        /// </summary>
        /// <param name="outputResults">Flag to define need to output.</param>
        /// <returns>Transformed configuration of the service.</returns>
        private TmdbServiceConfigurationInfo GetServiceConfiguration(bool outputResults)
        {
            var config = _tmdbClient.GetConfigAsync().Result;

            if (config.Images is null)
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
