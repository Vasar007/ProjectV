﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Acolyte.Assertions;
using Acolyte.Linq;

namespace ProjectV.DesktopApp.Models.ContentDirectories
{
    internal sealed class ContentDirectoryInfo
    {
        public string SourceDirectoryPath { get; }

        public ContentTypeToFind ContentType { get; }

        public IReadOnlyList<ContentInfo> ContentPaths { get; }

        public string DisplayString =>
            $"Found content of type {ContentType.ToString()} in directory: {SourceDirectoryPath}";


        public ContentDirectoryInfo(string sourceDirectoryPath, ContentTypeToFind contentType,
            IReadOnlyDictionary<string, IReadOnlyList<string>> data)
        {
            data.ThrowIfNull(nameof(data));

            SourceDirectoryPath =
                sourceDirectoryPath.ThrowIfNullOrEmpty(nameof(sourceDirectoryPath));
            ContentType = contentType;

            ContentPaths = TransformToContentModels(data);
        }

        public void PrintResultToOutput()
        {
            Debug.WriteLine($"Content type: {ContentType.ToString()}.");
            foreach (ContentInfo content in ContentPaths)
            {
                Debug.WriteLine(content.Title);

                Debug.WriteLine(
                    $"{string.Join($"{Environment.NewLine}", content.Paths)}{Environment.NewLine}"
                );
            }
        }

        private IReadOnlyList<ContentInfo> TransformToContentModels(
            IReadOnlyDictionary<string, IReadOnlyList<string>> data)
        {
            data.ThrowIfNull(nameof(data));

            return data
                .Select(datum => new ContentInfo(datum.Key, datum.Value))
                .ToReadOnlyList();
        }
    }
}
