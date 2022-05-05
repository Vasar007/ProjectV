using Acolyte.Assertions;
using Acolyte.Common;
using ProjectV.Models.WebServices.Responses;

namespace ProjectV.DesktopApp.Models.Things
{
    internal sealed class ThingResultInfo
    {
        public string ServiceName { get; }

        public Result<ProcessingResponse, ErrorResponse> Result { get; }


        public ThingResultInfo(
            string serviceName,
            Result<ProcessingResponse, ErrorResponse> result)
        {
            ServiceName = serviceName.ThrowIfNullOrWhiteSpace(nameof(serviceName));
            Result = result;
        }
    }
}
