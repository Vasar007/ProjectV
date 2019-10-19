using ThingAppraiser.Extensions;

namespace ThingAppraiser.DesktopApp.Models.ContentDirectories
{
    internal sealed class ContentDirectoryParametersInfo
    {
        public string DirectoryPath { get; }
        
        public ContentTypeToFind ContentType { get; }


        public ContentDirectoryParametersInfo(string directoryPath, ContentTypeToFind contentType)
        {
            DirectoryPath = directoryPath.ThrowIfNullOrWhiteSpace(nameof(directoryPath));
            ContentType = contentType;
        }
    }
}
