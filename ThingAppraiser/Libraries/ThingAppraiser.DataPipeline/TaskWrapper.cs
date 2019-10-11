using System;
using System.Threading.Tasks;
using ThingAppraiser.Exceptions;
using ThingAppraiser.Extensions;
using ThingAppraiser.Logging;

namespace ThingAppraiser.DataPipeline
{
    internal static class TaskWrapper
    {
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor(typeof(TaskWrapper));

        public static Task<TResult> LogIfErrorOccured<TResult>(this Task<TResult> task)
        {
            task.ThrowIfNull(nameof(task));

            return task.ContinueWith(
                task =>
                {
                    switch (task.Status)
                    {
                        case TaskStatus.RanToCompletion:
                        {
                            return task.Result;
                        }

                        case TaskStatus.Faulted:
                        {
                            Exception exception =
                                ExceptionsHelper.UnwrapAggregateExceptionIfCan(task.Exception);

                            _logger.Error(exception, "Task is in the faulted state.");

                            throw new TaskFaultedException(
                                "Rethrow exception because a task is in the faulted state.",
                                exception
                            );
                        }

                        default:
                        {
                            return default!;
                        }
                    }
                }
            );
        }
    }
}
