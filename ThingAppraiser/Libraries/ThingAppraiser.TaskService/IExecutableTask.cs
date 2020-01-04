using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ThingAppraiser.IO.Input;
using ThingAppraiser.IO.Output;
using ThingAppraiser.Models.Internal;
using ThingAppraiser.Models.WebService;

namespace ThingAppraiser.TaskService
{
    public interface IExecutableTask
    {
        Guid Id { get; }

        int ExecutionsNumber { get; }

        TimeSpan DelayTime { get; }

        RestartPointKind RestartPoint { get; }

        Task<IReadOnlyList<ServiceStatus>> ExecuteAsync();

        Task<IReadOnlyList<ServiceStatus>> ExecuteAsync(RequestData requestData,
            IInputterAsync additionalInputterAsync, IOutputterAsync additionalOutputterAsync);
    }
}
