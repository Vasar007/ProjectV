using Prism.Events;

namespace ThingAppraiser.DesktopApp.Domain.Messages
{
    public sealed class SaveToplistMessage : PubSubEvent<string>
    {
        public SaveToplistMessage()
        {
        }
    }
}
