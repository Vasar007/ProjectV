using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ThingAppraiser.ContentDirectories;
using ThingAppraiser.Extensions;

namespace ThingAppraiser.DesktopApp.Models.ContentDirectories
{
    internal sealed class ContentFinderWrapper
    {
        public ContentFinderWrapper()
        {
        }

        public async Task<ContentDirectoryInfo> GetAllDirectoryContentAsync(
            string directoryPath, ContentTypeToFind contentType)
        {
            directoryPath.ThrowIfNullOrWhiteSpace(nameof(directoryPath));

            IEnumerable<Tuple<string, IEnumerable<string>>> enumerableResults = await ContentFinder
                .FindContentForDirAsync(directoryPath, contentType.ConvertToLibraryEnum())
                .ConfigureAwait(continueOnCapturedContext: false);

            IReadOnlyDictionary<string, IReadOnlyList<string>> result = ContentFinder
                .ConvertToReadOnly(enumerableResults);

            return new ContentDirectoryInfo(directoryPath, contentType, result);
        }
    }
}
