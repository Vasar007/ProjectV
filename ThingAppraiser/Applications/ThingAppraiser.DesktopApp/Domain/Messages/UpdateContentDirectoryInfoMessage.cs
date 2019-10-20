using Prism.Events;
using ThingAppraiser.DesktopApp.Models.ContentDirectories;

namespace ThingAppraiser.DesktopApp.Domain.Messages
{
    internal sealed class UpdateContentDirectoryInfoMessage : PubSubEvent<ContentDirectoryInfo>
    {
        public UpdateContentDirectoryInfoMessage()
        {
        }
    }
}
