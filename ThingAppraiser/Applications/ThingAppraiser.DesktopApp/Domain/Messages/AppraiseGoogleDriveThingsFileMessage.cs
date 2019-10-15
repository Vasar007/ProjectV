using Prism.Events;

namespace ThingAppraiser.DesktopApp.Domain.Messages
{
    internal sealed class AppraiseGoogleDriveThingsFileMessage : PubSubEvent<string>
    {
        public AppraiseGoogleDriveThingsFileMessage()
        {
        }
    }
}
