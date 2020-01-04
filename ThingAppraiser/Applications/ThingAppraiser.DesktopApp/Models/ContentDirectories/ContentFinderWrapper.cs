using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Acolyte.Assertions;
using ThingAppraiser.ContentDirectories;

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

            // TODO: get paging info parameters from View.
            IReadOnlyDictionary<string, IReadOnlyList<string>> result = await ContentFinder
                .FindContentForDirWithPagingAsync(
                    directoryName: directoryPath,
                    contentType: contentType.ConvertToLibraryEnum(),
                    pagingInfo: ContentModels.CreateOption(new ContentModels.PagingInfo(0, 10))
                )
                .ConfigureAwait(continueOnCapturedContext: false);

            return new ContentDirectoryInfo(directoryPath, contentType, result);
        }
    }
}
