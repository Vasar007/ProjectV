using System.Threading.Tasks;
using Acolyte.Assertions;
using Acolyte.Exceptions;
using ProjectV.Models.Internal.Tasks;

namespace ProjectV.DAL.EntityFramework
{
    public sealed class TaskInfoService : ITaskInfoService
    {
        private readonly ProjectVDbContext _context;

        private readonly IMapper<TaskInfo, TaskDbModel> _mapper;


        public TaskInfoService(
            ProjectVDbContext context)
        {
            _context = context.ThrowIfNull(nameof(context));

            _mapper = new TaskMapper();
        }

        #region ITaskRepository Implementation

        public async Task AddAsync(TaskInfo taskInfo)
        {
            var taskDbModel = _mapper.Map(taskInfo);
            await _context.GetTasksDbSet().AddAsync(taskDbModel);

            await _context.SaveChangesAsync();
        }

        public async Task<TaskInfo?> FindByIdAsync(TaskId taskId)
        {
            TaskDbModel? taskDbModel = await _context.GetTasksDbSet().FindAsync(taskId.Value);

            return _mapper.Map(taskDbModel);
        }

        public async Task<TaskInfo> GetByIdAsync(TaskId taskId)
        {
            TaskInfo? taskInfo = await FindByIdAsync(taskId);
            if (taskInfo is null)
            {
                throw new NotFoundException(
                    $"Failed to found task info with ID '{taskId.ToString()}'."
                );
            }

            return taskInfo;
        }

        #endregion
    }
}
