using System;
using System.Threading;
using System.Threading.Tasks;
using ThingAppraiser.Models.Data;
using ThingAppraiser.SteamService.Models;

namespace ThingAppraiser.SteamService
{
    public interface ISteamApiClient : IDisposable
    {
        /// <summary>
        /// Key to get access to Steam service (using only for client data requests).
        /// </summary>
        string ApiKey { get; }


        Task<SteamBriefInfoContainer> GetAppListAsync(CancellationToken token = default);

        Task<SteamGameInfo> TryGetSteamAppAsync(int appId, SteamCountryCode countryCode,
            SteamResponseLanguage language, CancellationToken token = default);
    }
}
