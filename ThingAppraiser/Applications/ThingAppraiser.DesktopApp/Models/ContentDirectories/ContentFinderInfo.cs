using System;
using System.Collections.Generic;
using System.Diagnostics;
using Prism.Mvvm;
using ThingAppraiser.Extensions;

namespace ThingAppraiser.DesktopApp.Models.ContentDirectories
{
    // TODO: implement model for content finder lib.
    internal sealed class ContentFinderInfo : BindableBase
    {
        public IReadOnlyDictionary<string, IReadOnlyList<string>> Data { get; }


        public ContentFinderInfo(IReadOnlyDictionary<string, IReadOnlyList<string>> data)
        {
            Data = data.ThrowIfNull(nameof(data));
        }

        public void PrintResultToOutput()
        {
            foreach ((string directoryName, IReadOnlyList<string> files) in Data)
            {
                Debug.WriteLine(directoryName);

                Debug.WriteLine(
                    $"{string.Join($"{Environment.NewLine}", files)}{Environment.NewLine}"
                );
            }
        }
    }
}
