using Prism.Events;

namespace ThingAppraiser.DesktopApp.Domain.Messages
{
    public sealed class SaveToplistToFileMessage : PubSubEvent<string>
    {
        public SaveToplistToFileMessage()
        {
        }
    }
}
