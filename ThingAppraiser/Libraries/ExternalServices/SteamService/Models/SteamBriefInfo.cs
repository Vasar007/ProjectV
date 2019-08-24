namespace ThingAppraiser.SteamService.Models
{
    public sealed class SteamBriefInfo
    {
        public int AppId { get; set; }

        public string Name { get; set; }


        public SteamBriefInfo(int appId, string name)
        {
            AppId = appId;
            Name = name.ThrowIfNull(nameof(name));
        }
    }
}
