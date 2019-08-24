using System;
using System.Collections.Generic;
using System.Linq;
using ThingAppraiser.Logging;
using ThingAppraiser.Communication;
using ThingAppraiser.Models.Data;
using ThingAppraiser.Models.Internal;
using ThingAppraiser.TmdbService;
using ThingAppraiser.TmdbService.Models;

namespace ThingAppraiser.Crawlers.Tmdb
{
    /// <summary>
    /// Concrete crawler for The Movie Database service.
    /// </summary>
    public sealed class TmdbCrawler : Crawler
    {
        /// <summary>
        /// Logger instance for current class.
        /// </summary>
        private static readonly ILogger _logger = LoggerFactory.CreateLoggerFor<TmdbCrawler>();

        /// <summary>
        /// Adapter class to make a calls to TMDb API.
        /// </summary>
        private readonly ITmdbClient _tmdbClient;

        /// <inheritdoc />
        public override string Tag { get; } = nameof(TmdbCrawler);

        /// <inheritdoc />
        public override Type TypeId { get; } = typeof(TmdbMovieInfo);


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
        public TmdbCrawler(string apiKey, int maxRetryCount)
        {
            apiKey.ThrowIfNullOrWhiteSpace(nameof(apiKey));

            _tmdbClient = TmdbClientFactory.CreateClient(apiKey, maxRetryCount);
        }

        #region Crawler Overridden Methods

        /// <inheritdoc />
        public override List<BasicInfo> GetResponse(List<string> entities, bool outputResults)
        {
            TmdbServiceConfiguration.SetServiceConfigurationIfNeed(
                GetServiceConfiguration(outputResults)
            );

            // Use HashSet to avoid duplicated data which can produce errors in further work.
            var searchResults = new HashSet<BasicInfo>();
            foreach (string movie in entities)
            {
                TmdbSearchContainer response = _tmdbClient.TrySearchMovieAsync(movie).Result;

                if (response is null || response.Results.IsNullOrEmpty())
                {
                    string message = $"{movie} was not processed.";
                    _logger.Warn(message);
                    GlobalMessageHandler.OutputMessage(message);

                    continue;
                }

                // Get first search result from response and ignore all the rest.
                TmdbMovieInfo searchResult = response.Results.First();
                if (outputResults)
                {
                    GlobalMessageHandler.OutputMessage($"Got {searchResult.Title} from \"{Tag}\".");
                }

                searchResults.Add(searchResult);
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
            TmdbServiceConfigurationInfo config = _tmdbClient.GetConfigAsync().Result;

            if (outputResults)
            {
                GlobalMessageHandler.OutputMessage("Got TMDb config.");
            }

            return config;
        }
    }
}
