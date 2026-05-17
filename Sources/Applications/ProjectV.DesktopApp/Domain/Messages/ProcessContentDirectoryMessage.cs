using Prism.Events;
using ProjectV.DesktopApp.Models.ContentDirectories;

namespace ProjectV.DesktopApp.Domain.Messages
{
    internal sealed class ProcessContentDirectoryMessage :
        PubSubEvent<ContentDirectoryParametersInfo>
    {
        public ProcessContentDirectoryMessage()
        {
        }
    }
}
