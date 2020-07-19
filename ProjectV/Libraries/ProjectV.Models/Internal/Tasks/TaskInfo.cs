using System.Text;
using Acolyte.Assertions;

namespace ProjectV.Models.Internal.Tasks
{
    public sealed class TaskInfo
    {
        public TaskId Id { get; }

        public string Name { get; }

        // TODO: wrap state with proper enum.
        public int State { get; }

        // TODO: wrap result with proper enum.
        public int Result { get; }

        public string Config { get; }


        public TaskInfo(
            TaskId id,
            string name,
            int state,
            int result,
            string config)
        {
            Id = id;
            Name = name.ThrowIfNullOrWhiteSpace(nameof(config));
            State = state;
            Result = result;
            Config = config.ThrowIfNullOrWhiteSpace(nameof(config));
        }

        public static TaskInfo Create(
            string name,
            string config)
        {
            return new TaskInfo(
               id: TaskId.Create(),
               name: name,
               state: 0,
               result: 0,
               config: config
           );
        }

        public string ToLogString()
        {
            var sb = new StringBuilder()
                .AppendLine($"[{nameof(TaskInfo)}]")
                .AppendLine($"Id: {Id.ToString()}")
                .AppendLine($"Name: {Name}")
                .AppendLine($"State: {State.ToString()}")
                .AppendLine($"Result: {Result.ToString()}")
                .AppendLine($"Config: '{Config.ToString()}'");

            return sb.ToString();
        }
    }
}
