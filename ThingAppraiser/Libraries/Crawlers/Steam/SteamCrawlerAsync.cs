using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using ThingAppraiser.Communication;
using ThingAppraiser.Logging;
using ThingAppraiser.Models.Data;
using ThingAppraiser.SteamService;
using ThingAppraiser.SteamService.Models;

namespace ThingAppraiser.Crawlers.Steam
{
    /// <summary>
    /// Provides async version of Steam crawler.
    /// </summary>
    public sealed class SteamCrawlerAsync : CrawlerAsync
    {
        /// <summary>
        /// Logger instance for current class.
        /// </summary>
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor<SteamCrawlerAsync>();

        /// <summary>
        /// Adapter class to make a calls to Steam API.
        /// </summary>
        private readonly ISteamApiClient _steamApiClient;

        /// <inheritdoc />
        public override string Tag { get; } = nameof(SteamCrawlerAsync);

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
        public SteamCrawlerAsync(string apiKey)
        {
            apiKey.ThrowIfNullOrWhiteSpace(nameof(apiKey));

            _steamApiClient = SteamApiClientFactory.CreateClient(apiKey);
        }

        #region CrawlerAsync Overridden Methods

        /// <inheritdoc />
        public override async Task<bool> GetResponse(BufferBlock<string> entitiesQueue,
            BufferBlock<BasicInfo> responsesQueue, bool outputResults)
        {
            if (SteamAppsStorage.IsEmpty)
            {
                SteamBriefInfoContainer steamApps = await _steamApiClient.GetAppListAsync();
                SteamAppsStorage.FillStorage(steamApps);
            }

            // Use HashSet to avoid duplicated data which can produce errors in further work.
            var searchResults = new HashSet<BasicInfo>();
            while (await entitiesQueue.OutputAvailableAsync())
            {
                string game = await entitiesQueue.ReceiveAsync();

                int? appId = SteamAppsStorage.TryGetAppIdByName(game);

                if (!appId.HasValue)
                {
                    string message = $"{game} was not find in Steam responses storage.";
                    _logger.Warn(message);
                    GlobalMessageHandler.OutputMessage(message);

                    continue;
                }

                var response = await _steamApiClient.TryGetSteamAppAsync(
                    appId.Value, SteamCountryCode.Russia, SteamResponseLanguage.English
                );

                if (response is null)
                {
                    string message = $"{game} was not processed.";
                    _logger.Warn(message);
                    GlobalMessageHandler.OutputMessage(message);

                    continue;
                }

                if (outputResults)
                {
                    GlobalMessageHandler.OutputMessage($"Got {response} from \"{Tag}\".");
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
