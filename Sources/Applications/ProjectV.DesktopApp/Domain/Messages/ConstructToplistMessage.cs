using Prism.Events;
using ProjectV.DesktopApp.Models.Toplists;

namespace ProjectV.DesktopApp.Domain.Messages
{
    internal sealed class ConstructToplistMessage : PubSubEvent<ToplistParametersInfo>
    {
        public ConstructToplistMessage()
        {
        }
    }
}
