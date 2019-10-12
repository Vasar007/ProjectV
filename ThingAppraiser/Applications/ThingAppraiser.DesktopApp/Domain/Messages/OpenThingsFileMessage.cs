using Prism.Events;

namespace ThingAppraiser.DesktopApp.Domain.Messages
{
    internal sealed class OpenThingsFileMessage : PubSubEvent<string>
    {
        public OpenThingsFileMessage()
        {
        }
    }
}
