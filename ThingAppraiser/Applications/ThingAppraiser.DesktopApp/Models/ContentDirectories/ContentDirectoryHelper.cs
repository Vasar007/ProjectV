using System;
using ThingAppraiser.ContentDirectories;

namespace ThingAppraiser.DesktopApp.Models.ContentDirectories
{
    internal static class ContentDirectoryHelper
    {
        public static ContentFinder.ContentType ConvertToLibraryEnum(
            this ContentTypeToFind contentType)
        {
            return contentType switch
            {
                ContentTypeToFind.Movie => ContentFinder.ContentType.Movie,

                ContentTypeToFind.Image => ContentFinder.ContentType.Image,

                ContentTypeToFind.Text => ContentFinder.ContentType.Text,

                _ => throw new ArgumentOutOfRangeException(
                         nameof(contentType), contentType,
                         $"Unknown content type:\"{contentType.ToString()}\"."
                     )
            };
        }
    }
}
