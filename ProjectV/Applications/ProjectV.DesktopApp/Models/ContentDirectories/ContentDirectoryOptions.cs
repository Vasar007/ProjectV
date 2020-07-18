using System.Collections.Generic;

namespace ProjectV.DesktopApp.Models.ContentDirectories
{
    internal static class ContentDirectoryOptions
    {
        public static IReadOnlyList<ContentTypeToFind> ContentTypes { get; } = new List<ContentTypeToFind>
        {
            ContentTypeToFind.Movie,
            ContentTypeToFind.Image,
            ContentTypeToFind.Text
        };
    }
}
