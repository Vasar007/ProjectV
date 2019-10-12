using Prism.Events;

namespace ThingAppraiser.DesktopApp.Domain.Messages
{
    public sealed class OpenThingsFileMessage : PubSubEvent<string>
    {
        public OpenThingsFileMessage()
        {
        }
    }
}
