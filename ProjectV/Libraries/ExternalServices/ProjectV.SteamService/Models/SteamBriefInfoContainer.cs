using System.Collections.Generic;
using Acolyte.Assertions;

namespace ProjectV.SteamService.Models
{
    public sealed class SteamBriefInfoContainer
    {
        public IReadOnlyList<SteamBriefInfo> Results { get; }

        public SteamBriefInfoContainer(IReadOnlyList<SteamBriefInfo> results)
        {
            Results = results.ThrowIfNull(nameof(results));
        }
    }
}
