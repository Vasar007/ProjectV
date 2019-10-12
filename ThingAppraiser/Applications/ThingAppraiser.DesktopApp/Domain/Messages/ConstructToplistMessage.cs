using Prism.Events;
using ThingAppraiser.DesktopApp.Models.Toplists;

namespace ThingAppraiser.DesktopApp.Domain.Messages
{
    internal sealed class ConstructToplistMessage : PubSubEvent<ToplistParametersInfo>
    {
        public ConstructToplistMessage()
        {
        }
    }
}
