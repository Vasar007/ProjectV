using ProjectV.DataAccessLayer.Orm;
using ProjectV.Models.Configuration;

namespace ProjectV.ProcessingWebService.v1.Domain
{
    public interface ITargetServiceCreator
    {
        IServiceRequestProcessor CreateRequestProcessor(
            ServiceType serviceType, IJobInfoService jobInfoService
        );
    }
}
