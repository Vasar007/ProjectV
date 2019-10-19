using System.Collections.Generic;
using ThingAppraiser.ContentDirectories;
using ThingAppraiser.Extensions;

namespace ThingAppraiser.DesktopApp.Models.ContentDirectories
{
    internal sealed class ContentFinderWrapper
    {
        public ContentFinderWrapper()
        {
        }

        public ContentDirectoryInfo GetAllDirectoryContent(
            string directoryPath, ContentTypeToFind contentType)
        {
            directoryPath.ThrowIfNullOrWhiteSpace(nameof(directoryPath));

            IReadOnlyDictionary<string, IReadOnlyList<string>> result = ContentFinder
                .findContentForDir(directoryPath, contentType.ConvertToLibraryEnum())
                .ToReadOnlyDictionary(tuple => tuple.Item1, tuple => tuple.Item2.ToReadOnlyList());

            return new ContentDirectoryInfo(directoryPath, contentType, result);
        }
    }
}
