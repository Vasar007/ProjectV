using ThingAppraiser.Extensions;

namespace ThingAppraiser.DesktopApp.Models.Things
{
    internal sealed class ThingPerformerInfo
    {
        public string ServiceName { get; }

        public ThingsDataToAppraise Data { get; }


        public ThingPerformerInfo(string serviceName, ThingsDataToAppraise thingsData)
        {
            ServiceName = serviceName.ThrowIfNullOrWhiteSpace(nameof(serviceName));
            Data = thingsData.ThrowIfNull(nameof(thingsData));
        }
    }
}
