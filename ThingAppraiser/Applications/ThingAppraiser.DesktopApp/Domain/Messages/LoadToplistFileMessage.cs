using Prism.Events;

namespace ThingAppraiser.DesktopApp.Domain.Messages
{
    internal sealed class LoadToplistFileMessage : PubSubEvent<string>
    {
        public LoadToplistFileMessage()
        {
        }
    }
}
