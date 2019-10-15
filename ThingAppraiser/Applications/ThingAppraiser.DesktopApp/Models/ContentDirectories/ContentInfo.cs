using System.Collections.Generic;
using ThingAppraiser.Extensions;

namespace ThingAppraiser.DesktopApp.Models.ContentDirectories
{
    internal sealed class ContentInfo
    {
        public string Title { get; }

        public IReadOnlyList<string> Paths { get; }


        public ContentInfo(string title, IReadOnlyList<string> paths)
        {
            paths.ThrowIfNullOrEmpty(nameof(paths));

            Title = title.ThrowIfNullOrEmpty(nameof(title));
            Paths = paths;
        }
    }
}
