using System.Collections.Generic;

namespace ProjectV.DesktopApp.Models.Toplists
{
    internal static class ToplistOptions
    {
        public static IReadOnlyList<ToplistType> ToplistTypes { get; } = new List<ToplistType>
        {
            ToplistType.Score,
            ToplistType.Simple
        };

        public static IReadOnlyList<ToplistFormat> ToplistFormats { get; } = new List<ToplistFormat>
        {
            ToplistFormat.Forward,
            ToplistFormat.Reverse
        };
    }
}
