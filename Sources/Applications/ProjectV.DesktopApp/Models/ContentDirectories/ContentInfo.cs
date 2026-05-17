using System.Collections.Generic;
using System.Linq;
using Acolyte.Assertions;
using Acolyte.Linq;

namespace ProjectV.DesktopApp.Models.ContentDirectories
{
    internal sealed class ContentInfo
    {
        public string Title { get; }

        public IReadOnlyList<ContentPath> Paths { get; }


        public ContentInfo(string title, IReadOnlyList<string> paths)
        {
            paths.ThrowIfNullOrEmpty(nameof(paths));

            Title = title.ThrowIfNullOrEmpty(nameof(title));

            Paths = paths
                .Select(paths => new ContentPath(paths))
                .ToReadOnlyList();
        }
    }
}
