using Acolyte.Assertions;

namespace ProjectV.SteamService.Models
{
    public sealed class SteamBriefInfo
    {
        public int AppId { get; }

        public string Name { get; }


        public SteamBriefInfo(int appId, string name)
        {
            AppId = appId;
            Name = name.ThrowIfNull(nameof(name));
        }
    }
}
