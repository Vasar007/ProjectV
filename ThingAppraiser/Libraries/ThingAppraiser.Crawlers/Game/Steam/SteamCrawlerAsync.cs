﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using ThingAppraiser.Communication;
using ThingAppraiser.Extensions;
using ThingAppraiser.Logging;
using ThingAppraiser.Models.Data;
using ThingAppraiser.SteamService;
using ThingAppraiser.SteamService.Models;

namespace ThingAppraiser.Crawlers.Game.Steam
{
    /// <summary>
    /// Provides async version of Steam crawler.
    /// </summary>
    public sealed class SteamCrawlerAsync : ICrawlerAsync, ICrawlerBase, IDisposable, ITagable,
        ITypeId
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

        /// <summary>
        /// Boolean flag used to show that object has already been disposed.
        /// </summary>
        private bool _disposed;

        #region ITagable Implementation

        /// <inheritdoc />
        public string Tag { get; } = nameof(SteamCrawler);

        #endregion

        #region ITypeId Implementation

        /// <inheritdoc />
        public Type TypeId { get; } = typeof(SteamGameInfo);

        #endregion


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
        public async Task<bool> GetResponse(ISourceBlock<string> entitiesQueue,
            ITargetBlock<BasicInfo> responsesQueue, bool outputResults)
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

        #region IDisposable Implementation

        /// <summary>
        /// Releases resources of TMDb client.
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            _steamApiClient.Dispose();
        }

        #endregion
    }
}
