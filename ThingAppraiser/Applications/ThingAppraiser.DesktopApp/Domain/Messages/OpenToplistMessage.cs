using Prism.Events;

namespace ThingAppraiser.DesktopApp.Domain.Messages
{
    public sealed class OpenToplistMessage : PubSubEvent<string>
    {
        public OpenToplistMessage()
        {
        }
    }
}
