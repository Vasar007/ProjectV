using ProjectV.Models.Internal.Tasks;

namespace ProjectV.DAL.EntityFramework
{
    internal sealed class TaskMapper : IMapper<TaskInfo, TaskDbModel>
    {
        public TaskMapper()
        {
        }

        #region IMapper<TaskInfo, TaskDbModel> Implementation

        public TaskInfo Map(TaskDbModel taskDbModel)
        {
            return new TaskInfo(
                id: TaskId.Wrap(taskDbModel.Id),
                name: taskDbModel.Name,
                state: taskDbModel.State,
                result: taskDbModel.Result,
                config: taskDbModel.Config
            );
        }

        public TaskDbModel Map(TaskInfo taskInfo)
        {
            return new TaskDbModel(
                id: taskInfo.Id.Value,
                name: taskInfo.Name,
                state: taskInfo.State,
                result: taskInfo.Result,
                config: taskInfo.Config
            );
        }

        #endregion
    }
}
