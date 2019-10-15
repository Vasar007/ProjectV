using System.Collections.Generic;
using Prism.Events;

namespace ThingAppraiser.DesktopApp.Domain.Messages
{
    internal sealed class AppraiseInputThingsMessage : PubSubEvent<IReadOnlyList<string>>
    {
        public AppraiseInputThingsMessage()
        {
        }
    }
}
