using System;
using ProjectV.ContentDirectories;

namespace ProjectV.DesktopApp.Models.ContentDirectories
{
    internal static class ContentDirectoryHelper
    {
        public static ContentModels.ContentType ConvertToLibraryEnum(
            this ContentTypeToFind contentType)
        {
            return contentType switch
            {
                ContentTypeToFind.Movie => ContentModels.ContentType.Movie,

                ContentTypeToFind.Image => ContentModels.ContentType.Image,

                ContentTypeToFind.Text => ContentModels.ContentType.Text,

                _ => throw new ArgumentOutOfRangeException(
                         nameof(contentType), contentType,
                         $"Unknown content type: \"{contentType.ToString()}\"."
                     )
            };
        }
    }
}
