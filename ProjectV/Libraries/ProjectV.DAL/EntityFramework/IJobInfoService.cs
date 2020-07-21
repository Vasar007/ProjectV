using System.Threading.Tasks;
using ProjectV.Models.Internal.Jobs;

namespace ProjectV.DAL.EntityFramework
{
    public interface IJobInfoService
    {
        Task AddAsync(JobInfo jobInfo);

        Task<JobInfo?> FindByIdAsync(JobId jobId);

        Task<JobInfo> GetByIdAsync(JobId jobId);
    }
}
