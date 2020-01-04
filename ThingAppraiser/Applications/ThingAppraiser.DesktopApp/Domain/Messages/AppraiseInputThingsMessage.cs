using System.Collections.Generic;
using Prism.Events;
using ThingAppraiser.DesktopApp.Models.Things;

namespace ThingAppraiser.DesktopApp.Domain.Messages
{
    internal sealed class AppraiseInputThingsMessage : PubSubEvent<ThingsDataToAppraise>
    {
        public AppraiseInputThingsMessage()
        {
        }
    }
}
