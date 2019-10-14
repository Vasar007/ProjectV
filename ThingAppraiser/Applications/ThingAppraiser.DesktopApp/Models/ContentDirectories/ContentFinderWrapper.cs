using System;
using System.Collections.Generic;
using System.Diagnostics;
using ThingAppraiser.ContentDirectories;
using ThingAppraiser.Extensions;

namespace ThingAppraiser.DesktopApp.Models.ContentDirectories
{
    internal sealed class ContentFinderWrapper
    {
        public ContentFinderWrapper()
        {
        }

        public IReadOnlyDictionary<string, IReadOnlyList<string>> GetAllDirectoryContent(
            string directoryPath, ContentTypeToFind contentType)
        {
            directoryPath.ThrowIfNullOrWhiteSpace(nameof(directoryPath));

            IReadOnlyDictionary<string, IReadOnlyList<string>> result = ContentFinder
                .findContentForDir(directoryPath, contentType.ConvertToLibraryEnum())
                .ToReadOnlyDictionary(tuple => tuple.Item1, tuple => tuple.Item2.ToReadOnlyList());

            return result;
        }

        public void PrintResultToOutput(IReadOnlyDictionary<string, IReadOnlyList<string>> result)
        {
            result.ThrowIfNull(nameof(result));

            foreach ((string directoryName, IReadOnlyList<string> files) in result)
            {
                Debug.WriteLine(directoryName);

                Debug.WriteLine(
                    $"{string.Join($"{Environment.NewLine}", files)}{Environment.NewLine}"
                );
            }
        }
    }
}
