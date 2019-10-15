using Prism.Events;
using ThingAppraiser.DesktopApp.Models.Things;

namespace ThingAppraiser.DesktopApp.Domain.Messages
{
    internal sealed class AppraiseLocalThingsFileMessage : PubSubEvent<ThingsDataToAppraise>
    {
        public AppraiseLocalThingsFileMessage()
        {
        }
    }
}
