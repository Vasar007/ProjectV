using ThingAppraiser.Models.Configuration;

namespace ThingAppraiser.ProcessingWebService.v1.Domain
{
    public interface ITargetServiceCreator
    {
        IServiceRequestProcessor CreateRequestProcessor(ServiceType serviceType);
    }
}
