using Acolyte.Assertions;

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
            ToplistName = toplistName.ThrowIfNullOrWhiteSpace(nameof(toplistName));
            ToplistType = toplistType.ThrowIfEnumValueIsUndefined(nameof(toplistType));
            ToplistFormat = toplistFormat.ThrowIfEnumValueIsUndefined(nameof(toplistFormat));
        }
    }
}
