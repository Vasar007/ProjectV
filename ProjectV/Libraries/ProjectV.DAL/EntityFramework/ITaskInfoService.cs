using System.Threading.Tasks;
using ProjectV.Models.Internal.Tasks;

namespace ProjectV.DAL.EntityFramework
{
    public interface ITaskInfoService
    {
        Task AddAsync(TaskInfo taskInfo);

        Task<TaskInfo?> FindByIdAsync(TaskId taskId);

        Task<TaskInfo> GetByIdAsync(TaskId taskId);
    }
}
