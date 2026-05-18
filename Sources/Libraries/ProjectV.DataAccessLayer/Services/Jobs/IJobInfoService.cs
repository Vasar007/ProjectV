using ProjectV.DataAccessLayer.Services.Basic;
using ProjectV.Models.Internal.Jobs;

namespace ProjectV.DataAccessLayer.Services.Jobs
{
    public interface IJobInfoService : IDataInfoServiceBase<JobId, JobInfo>
    {
    }
}
