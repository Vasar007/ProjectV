using System;
using ThingAppraiser.Models.Configuration;

namespace ThingAppraiser.ProcessingWebService.v1.Domain
{
    public sealed class TargetServiceCreator : ITargetServiceCreator
    {
        public TargetServiceCreator()
        {
        }

        #region ITargetServiceCreator Implementation

        public IServiceRequestProcessor CreateRequestProcessor(ServiceType serviceType)
        {
            return serviceType switch
            {
                ServiceType.Sequential => new ServiceRequestProcessor(),

                ServiceType.TplDataflow => new ServiceAsyncRequestProcessor(),

                _ => throw new ArgumentOutOfRangeException(nameof(serviceType),
                                                           "Not known service type")
            };
        }

        #endregion
    }
}
