using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ThingAppraiser.Configuration;
using ThingAppraiser.Core;
using ThingAppraiser.Core.ShellBuilders;
using ThingAppraiser.Extensions;
using ThingAppraiser.IO.Input;
using ThingAppraiser.IO.Output;
using ThingAppraiser.Models.Internal;
using ThingAppraiser.Models.WebService;

namespace ThingAppraiser.TaskService
{
    public sealed class SimpleTask : IExecutableTask
    {
        public Guid Id { get; }

        public int ExecutionsNumber { get; }

        public TimeSpan DelayTime { get; }

        public RestartPointKind RestartPoint { get; } = RestartPointKind.None;


        public SimpleTask(Guid id, int executionsNumber, TimeSpan delayTime)
        {
            if (executionsNumber <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(executionsNumber), executionsNumber,
                                                      "Executions number must be positive.");
            }

            Id = id.ThrowIfEmpty(nameof(id));
            ExecutionsNumber = executionsNumber;
            DelayTime = delayTime;
        }

        public Task<IReadOnlyList<ServiceStatus>> ExecuteAsync()
        {
            // Take config from DB.
            // Create Shell.
            // Execute Shell with data.
            throw new NotImplementedException(
                "Current serivce version cannot have tasks with data in DB."
            );
        }

        public Task<IReadOnlyList<ServiceStatus>> ExecuteAsync(RequestData requestData,
             IInputterAsync additionalInputterAsync, IOutputterAsync additionalOutputterAsync)
        {
            ShellAsyncBuilderDirector builderDirector = ShellAsync.CreateBuilderDirector(
               XmlConfigCreator.TransformConfigToXDocument(requestData.ConfigurationXml)
            );
            using ShellAsync shell = builderDirector.MakeShell();

            shell.InputManagerAsync.Add(additionalInputterAsync);
            shell.OutputManagerAsync.Add(additionalOutputterAsync);

            return ExecuteSpecifiedNumberOfTimes(shell);
        }

        private async Task<IReadOnlyList<ServiceStatus>> ExecuteSpecifiedNumberOfTimes(
            ShellAsync shell)
        {
            var statuses = new List<ServiceStatus>(ExecutionsNumber);

            int executedCount = 0;
            while (true)
            {
                ServiceStatus status = await shell.Run("Processing request data.");
                statuses.Add(status);

                ++executedCount;

                if (executedCount == ExecutionsNumber) break;

                await Task.Delay(DelayTime);
            }

            return statuses;
        }
    }
}
