using System.Threading.Tasks;
using ProjectV.Models.Internal.Jobs;

namespace ProjectV.DAL.EntityFramework
{
    public interface IJobInfoService
    {
        Task AddAsync(JobInfo taskInfo);

        Task<JobInfo?> FindByIdAsync(JobId taskId);

        Task<JobInfo> GetByIdAsync(JobId taskId);
    }
}
