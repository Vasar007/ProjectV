using System;
using System.Threading;
using System.Threading.Tasks;
using SteamWebApiLib;
using SteamWebApiLib.Models.AppDetails;
using SteamWebApiLib.Models.BriefInfo;
using ThingAppraiser.Extensions;
using ThingAppraiser.Logging;
using ThingAppraiser.Models.Data;
using ThingAppraiser.SteamService.Mappers;
using ThingAppraiser.SteamService.Models;

namespace ThingAppraiser.SteamService
{
    internal sealed class SteamApiClient : ISteamApiClient, IDisposable
    {
        /// <summary>
        /// Logger instance for current class.
        /// </summary>
        private static readonly ILogger _logger = LoggerFactory.CreateLoggerFor<SteamApiClient>();

        /// <summary>
        /// Helper class to transform raw DTO objects to concrete object without extra data.
        /// </summary>
        private readonly IDataMapper<SteamAppBriefInfoList, SteamBriefInfoContainer>
            _containerMapper = new DataMapperSteamContainer();

        /// <summary>
        /// Helper class to transform raw DTO objects to concrete object without extra data.
        /// </summary>
        private readonly IDataMapper<SteamApp, SteamGameInfo> _dataMapper =
            new DataMapperSteamGame();

#pragma warning disable IDE0069, CA2213 // Disposable fields should be disposed
        /// <summary>
        /// Third-party helper class to make a calls to Steam API.
        /// </summary>
        private readonly SteamWebApiLib.SteamApiClient _steamApiClient;
#pragma warning restore IDE0069, CA2213 // Disposable fields should be disposed

        private bool _disposed;

        /// <inheritdoc />
        public string ApiKey { get; }
        

        public SteamApiClient(string apiKey)
        {
            ApiKey = apiKey.ThrowIfNullOrWhiteSpace(nameof(apiKey));

            _steamApiClient = new SteamWebApiLib.SteamApiClient(
                new SteamApiConfig
                {
                    ApiKey = apiKey
                }
            );
        }

        public async Task<SteamBriefInfoContainer> GetAppListAsync(
            CancellationToken token = default)
        {
            SteamAppBriefInfoList steamAppsList = await _steamApiClient.GetAppListAsync(token);

            return _containerMapper.Transform(steamAppsList);
        }

        public async Task<SteamGameInfo?> TryGetSteamAppAsync(int appId,
            SteamCountryCode countryCode, SteamResponseLanguage language,
            CancellationToken token = default)
        {
            _logger.Info($"Getting Steam game by app ID: \"{appId.ToString()}\" [countryCode: " +
                         $"{countryCode.ToString()}, language: {language.ToString()}].");

            try
            {
                SteamApp response = await _steamApiClient.GetSteamAppAsync(
                    appId, (CountryCode) countryCode, (Language) language, token
                );

                return _dataMapper.Transform(response);
            }
            catch (Exception ex)
            {
                _logger.Warn(ex, "Exception occurred during processing response for " +
                                 $"\"{appId.ToString()}\".");

                return null;
            }
        }

        #region IDisposable Implementation

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            // Disposing TMDb client leads to multiple task cancelled exceptions.
            // TODO: need to dig deeper in official docs about disposing TMDb client.
            // May be it somehow would be connected with similar issue with Steam API client.
            //_steamApiClient.Dispose();
        }

        #endregion
    }
}
