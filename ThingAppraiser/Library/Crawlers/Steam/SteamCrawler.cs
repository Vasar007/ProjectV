using System;
using System.Collections.Generic;
using System.Linq;
using SteamWebApiLib;
using SteamWebApiLib.Models.AppDetails;
using SteamWebApiLib.Models.BriefInfo;
using ThingAppraiser.Communication;
using ThingAppraiser.Logging;
using ThingAppraiser.Models.Data;

namespace ThingAppraiser.Crawlers.Steam
{
    /// <summary>
    /// Concrete crawler for Steam service.
    /// </summary>
    public class SteamCrawler : Crawler
    {
        /// <summary>
        /// Logger instance for current class.
        /// </summary>
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor<SteamCrawler>();

        /// <summary>
        /// Helper class to transform raw DTO objects to concrete object without extra data.
        /// </summary>
        private readonly IDataMapper<SteamApp, SteamGameInfo> _dataMapper =
            new DataMapperSteamGame(); 

        /// <summary>
        /// Key to get access to Steam service (using only for client data requests).
        /// </summary>
        private readonly string _apiKey;

        /// <summary>
        /// Third-party helper class to make a calls to Steam API.
        /// </summary>
        private readonly SteamApiClient _steamApiClient;

        /// <inheritdoc />
        public override string Tag { get; } = nameof(SteamCrawler);

        /// <inheritdoc />
        public override Type TypeId { get; } = typeof(SteamGameInfo);


        /// <summary>
        /// Initializes instance according to parameter values.
        /// </summary>
        /// <param name="apiKey">Key to get access to Steam service.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="apiKey" /> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="apiKey" /> presents empty strings or contains only whitespaces.
        /// </exception>
        public SteamCrawler(string apiKey)
        {
            _apiKey = apiKey.ThrowIfNullOrWhiteSpace(nameof(apiKey));

            _steamApiClient = new SteamApiClient(
                new SteamApiConfig
                {
                    ApiKey = _apiKey
                }
            );
        }

        #region Crawler Overridden Methods

        /// <inheritdoc />
        public override List<BasicInfo> GetResponse(List<string> entities, bool outputResults)
        {
            if (SteamAppsStorage.IsEmpty)
            {
                SteamAppBriefInfoList steamAppsList = _steamApiClient.GetAppListAsync().Result;
                SteamAppsStorage.FillStorage(steamAppsList);
            }

            // Use HashSet to avoid duplicated data which can produce errors in further work.
            var searchResults = new HashSet<BasicInfo>();
            foreach (string game in entities)
            {
                SteamApp response;
                try
                {
                    int appId = SteamAppsStorage.GetAppIdByName(game);
                    response = _steamApiClient.GetSteamAppAsync(
                        appId, CountryCode.Russia, Language.English
                    ).Result;
                }
                catch (Exception ex)
                {
                    _logger.Warn(ex, $"{game} wasn't processed.");
                    GlobalMessageHandler.OutputMessage($"{game} wasn't processed.");
                    continue;
                }

                if (outputResults)
                {
                    GlobalMessageHandler.OutputMessage($"Got {response} from {Tag}");
                }

                SteamGameInfo extractedInfo = _dataMapper.Transform(response);
                searchResults.Add(extractedInfo);
            }
            return searchResults.ToList();
        }

        #endregion
    }
}
