using Prism.Events;
using ProjectV.DesktopApp.Models.ContentDirectories;

namespace ProjectV.DesktopApp.Domain.Messages
{
    internal sealed class UpdateContentDirectoryInfoMessage : PubSubEvent<ContentDirectoryInfo>
    {
        public UpdateContentDirectoryInfoMessage()
        {
        }
    }
}
