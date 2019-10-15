using Prism.Events;

namespace ThingAppraiser.DesktopApp.Domain.Messages
{
    internal sealed class AppraiseLocalThingsFileMessage : PubSubEvent<string>
    {
        public AppraiseLocalThingsFileMessage()
        {
        }
    }
}
