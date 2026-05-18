using Prism.Events;
using ProjectV.DesktopApp.Models.Things;

namespace ProjectV.DesktopApp.Domain.Messages
{
    internal sealed class AppraiseInputThingsMessage : PubSubEvent<ThingsDataToAppraise>
    {
        public AppraiseInputThingsMessage()
        {
        }
    }
}
