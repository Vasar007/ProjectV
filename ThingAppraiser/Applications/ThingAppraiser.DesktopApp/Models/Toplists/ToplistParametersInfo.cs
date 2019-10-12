using ThingAppraiser.Extensions;

namespace ThingAppraiser.DesktopApp.Models.Toplists
{
    internal sealed class ToplistParametersInfo
    {
        public string ToplistName { get; }

        public ToplistType ToplistType { get; }

        public ToplistFormat ToplistFormat { get; }


        public ToplistParametersInfo(string toplistName, ToplistType toplistType,
            ToplistFormat toplistFormat)
        {
            ToplistName = toplistName.ThrowIfNullOrEmpty(nameof(toplistName));
            ToplistType = toplistType;
            ToplistFormat = toplistFormat;
        }
    }
}
