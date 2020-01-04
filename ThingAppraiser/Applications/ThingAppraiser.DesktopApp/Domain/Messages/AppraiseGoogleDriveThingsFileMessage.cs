using Prism.Events;
using ThingAppraiser.DesktopApp.Models.Things;

namespace ThingAppraiser.DesktopApp.Domain.Messages
{
    internal sealed class AppraiseGoogleDriveThingsFileMessage : PubSubEvent<ThingsDataToAppraise>
    {
        public AppraiseGoogleDriveThingsFileMessage()
        {
        }
    }
}
