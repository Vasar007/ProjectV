using Prism.Events;

namespace ProjectV.DesktopApp.Domain.Messages
{
    internal sealed class LoadToplistFileMessage : PubSubEvent<string>
    {
        public LoadToplistFileMessage()
        {
        }
    }
}
