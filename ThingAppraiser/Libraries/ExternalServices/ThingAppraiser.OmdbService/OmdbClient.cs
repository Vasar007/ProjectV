using System;
using System.Threading.Tasks;
using OMDbApiNet;
using OMDbApiNet.Model;
using ThingAppraiser.Logging;
using ThingAppraiser.Models.Data;
using ThingAppraiser.OmdbService.Mappers;

namespace ThingAppraiser.OmdbService
{
    internal sealed class OmdbClient : IOmdbClient, IDisposable
    {
        /// <summary>
        /// Logger instance for current class.
        /// </summary>
        private static readonly ILogger _logger = LoggerFactory.CreateLoggerFor<OmdbClient>();

        /// <summary>
        /// Helper class to transform raw DTO objects to concrete object without extra data.
        /// </summary>
        private readonly IDataMapper<Item, OmdbMovieInfo> _dataMapper =
            new DataMapperOmdbMovie();

        /// <summary>
        /// Third-party helper class to make a calls to OMDb API.
        /// </summary>
        private readonly AsyncOmdbClient _omdbClient;

        /// <inheritdoc />
        public string ApiKey { get; }

        public bool UseRottenTomatoesRatings { get; }


        public OmdbClient(string apikey, bool rottenTomatoesRatings = false)
        {
            ApiKey = apikey.ThrowIfNullOrWhiteSpace(nameof(apikey));
            UseRottenTomatoesRatings = rottenTomatoesRatings;

            _omdbClient = new AsyncOmdbClient(apikey, rottenTomatoesRatings);
        }

        public async Task<OmdbMovieInfo?> TryGetItemByTitleAsync(string title,
            bool fullPlot = false)
        {
            title.ThrowIfNullOrWhiteSpace(nameof(title));

            _logger.Info($"Getting OMDb item by title \"{title}\" " +
                         $"[fullPlot: {fullPlot.ToString()}].");

            Item response;
            try
            {
                response = await _omdbClient.GetItemByTitleAsync(title, fullPlot);
            }
            catch (Exception ex)
            {
                _logger.Warn(ex, $"Exception occurred during processing response for \"{title}\".");

                return null;
            }

            if (!string.Equals(response.Response, "True",
                               StringComparison.InvariantCultureIgnoreCase))
            {
                _logger.Warn($"Got response for \"{title}\" with negative result.");

                return null;
            }

            return _dataMapper.Transform(response);
        }

        #region IDisposable Implementation

        public void Dispose()
        {
            // Nothing to dispose.
        }

        #endregion
    }
}
