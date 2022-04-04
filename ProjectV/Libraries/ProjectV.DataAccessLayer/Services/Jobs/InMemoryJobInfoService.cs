using ProjectV.DataAccessLayer.Services.Basic;
using ProjectV.Models.Internal.Jobs;

namespace ProjectV.DataAccessLayer.Services.Jobs
{
    public sealed class InMemoryJobInfoService :
        InMemoryDataService<JobId, JobInfo>, IJobInfoService
    {
        public InMemoryJobInfoService()
        {
        }
    }
}
