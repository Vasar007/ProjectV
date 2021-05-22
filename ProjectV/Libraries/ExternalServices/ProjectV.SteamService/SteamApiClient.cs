using System;
using System.Threading;
using System.Threading.Tasks;
using Acolyte.Assertions;
using ProjectV.Logging;
using ProjectV.Models.Data;
using ProjectV.SteamService.Mappers;
using ProjectV.SteamService.Models;
using SteamWebApiLib;
using SteamWebApiLib.Models.AppDetails;
using SteamWebApiLib.Models.BriefInfo;

namespace ProjectV.SteamService
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

        /// <summary>
        /// Third-party helper class to make a calls to Steam API.
        /// </summary>
        private readonly SteamWebApiLib.SteamApiClient _steamApiClient;

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

        /// <summary>
        /// Boolean flag used to show that object has already been disposed.
        /// </summary>
        private bool _disposed;

        public void Dispose()
        {
            if (_disposed) return;

            _steamApiClient.Dispose();

            _disposed = true;
        }

        #endregion
    }
}
