using ThingAppraiser.Extensions;
using ThingAppraiser.Models.WebService;

namespace ThingAppraiser.DesktopApp.Models.Things
{
    internal sealed class ThingResultInfo
    {
        public string ServiceName { get; }

        public ProcessingResponse? Response { get; }


        public ThingResultInfo(string serviceName, ProcessingResponse? response)
        {
            ServiceName = serviceName.ThrowIfNullOrWhiteSpace(nameof(serviceName));
            Response = response.ThrowIfNull(nameof(response));
        }
    }
}
