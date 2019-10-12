using Prism.Events;

namespace ThingAppraiser.DesktopApp.Domain.Messages
{
    public sealed class OpenToplistFileMessage : PubSubEvent<string>
    {
        public OpenToplistFileMessage()
        {
        }
    }
}
