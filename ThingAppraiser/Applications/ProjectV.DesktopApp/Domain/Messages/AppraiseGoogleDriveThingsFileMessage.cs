using Prism.Events;
using ProjectV.DesktopApp.Models.Things;

namespace ProjectV.DesktopApp.Domain.Messages
{
    internal sealed class AppraiseGoogleDriveThingsFileMessage : PubSubEvent<ThingsDataToAppraise>
    {
        public AppraiseGoogleDriveThingsFileMessage()
        {
        }
    }
}
