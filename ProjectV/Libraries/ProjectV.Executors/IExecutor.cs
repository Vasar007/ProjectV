using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectV.IO.Input;
using ProjectV.IO.Output;
using ProjectV.Models.Internal;
using ProjectV.Models.Internal.Jobs;
using ProjectV.Models.WebService.Responses;

namespace ProjectV.Executors
{
    public interface IExecutor
    {
        JobId Id { get; }

        int ExecutionsNumber { get; }

        TimeSpan DelayTime { get; }

        RestartPointKind RestartPoint { get; }


        Task<IReadOnlyList<ServiceStatus>> ExecuteAsync();

        Task<IReadOnlyList<ServiceStatus>> ExecuteAsync(StartJobDataResponce jobData,
            IInputter additionalInputterAsync, IOutputter additionalOutputterAsync);
    }
}
