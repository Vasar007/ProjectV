using Acolyte.Assertions;
using ProjectV.Models.WebService;

namespace ProjectV.DesktopApp.Models.Things
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
