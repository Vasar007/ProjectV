using Prism.Events;

namespace ThingAppraiser.DesktopApp.Domain.Messages
{
    internal sealed class ProcessContentDirectoryMessage : PubSubEvent<string>
    {
        public ProcessContentDirectoryMessage()
        {
        }
    }
}
