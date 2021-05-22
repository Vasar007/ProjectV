using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Acolyte.Assertions;
using ProjectV.Configuration;
using ProjectV.Core;
using ProjectV.IO.Input;
using ProjectV.IO.Output;
using ProjectV.Models.Internal;
using ProjectV.Models.Internal.Jobs;
using ProjectV.Models.WebService;

namespace ProjectV.Executors
{
    public sealed class SimpleExecutor : IExecutor
    {
        private readonly JobInfo _jobInfo;

        public JobId Id => _jobInfo.Id;

        public int ExecutionsNumber { get; }

        public TimeSpan DelayTime { get; }

        public RestartPointKind RestartPoint { get; } = RestartPointKind.None;


        public SimpleExecutor(
            JobInfo jobInfo,
            int executionsNumber,
            TimeSpan delayTime)
        {
            if (executionsNumber <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(executionsNumber), executionsNumber,
                                                      "Executions number must be positive.");
            }

            _jobInfo = jobInfo.ThrowIfNull(nameof(jobInfo));
            ExecutionsNumber = executionsNumber;
            DelayTime = delayTime;
        }

        public Task<IReadOnlyList<ServiceStatus>> ExecuteAsync()
        {
            // Take config from DB.
            // Create Shell.
            // Execute Shell with data.
            throw new NotImplementedException(
                "Current service version cannot have job with data in DB."
            );
        }

        // TODO: remove this method later.
        public async Task<IReadOnlyList<ServiceStatus>> ExecuteAsync(RequestData requestData,
             IInputter additionalInputter, IOutputter additionalOutputter)
        {
            var builderDirector = Shell.CreateBuilderDirector(
               XmlConfigCreator.TransformConfigToXDocument(requestData.ConfigurationXml)
            );
            using Shell shell = builderDirector.MakeShell();

            shell.InputManager.Add(additionalInputter);
            shell.OutputManager.Add(additionalOutputter);

            return await ExecuteSpecifiedNumberOfTimes(shell);
        }

        private async Task<IReadOnlyList<ServiceStatus>> ExecuteSpecifiedNumberOfTimes(
            Shell shell)
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
