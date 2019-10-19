using System;
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

            IEnumerable<Tuple<string, IEnumerable<string>>> enumerableResults = ContentFinder
                .FindContentForDir(directoryPath, contentType.ConvertToLibraryEnum());

            IReadOnlyDictionary<string, IReadOnlyList<string>> result = ContentFinder
                .ConvertToReadOnly(enumerableResults);

            return new ContentDirectoryInfo(directoryPath, contentType, result);
        }
    }
}
