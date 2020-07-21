using System;
using ProjectV.DAL.EntityFramework;
using ProjectV.Models.Configuration;

namespace ProjectV.ProcessingWebService.v1.Domain
{
    public sealed class TargetServiceCreator : ITargetServiceCreator
    {
        public TargetServiceCreator()
        {
        }

        #region ITargetServiceCreator Implementation

        public IServiceRequestProcessor CreateRequestProcessor(
            ServiceType serviceType, IJobInfoService jobInfoService)
        {
            return serviceType switch
            {
                ServiceType.Sequential => new ServiceRequestProcessor(),

                ServiceType.TplDataflow => new ServiceAsyncRequestProcessor(jobInfoService),

                _ => throw new ArgumentOutOfRangeException(nameof(serviceType),
                                                           "Not known service type")
            };
        }

        #endregion
    }
}
