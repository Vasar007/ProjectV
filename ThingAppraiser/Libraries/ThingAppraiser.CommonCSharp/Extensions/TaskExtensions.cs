using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ThingAppraiser.Extensions
{
    public static class TaskExtensions
    {
        public static async Task<ResultOrException<TResult>> WrapResultOrExceptionAsync<TResult>(
            this Task<TResult> task)
        {
            try
            {
                TResult taskResult = await task;
                return new ResultOrException<TResult>(taskResult);
            }
            catch (Exception ex)
            {
                return new ResultOrException<TResult>(ex);
            }
        }

        public static async Task<ResultOrException<NoneResult>> WrapResultOrExceptionAsync(
            this Task task)
        {
            try
            {
                await task;
                return new ResultOrException<NoneResult>(new NoneResult());
            }
            catch (Exception ex)
            {
                return new ResultOrException<NoneResult>(ex);
            }
        }

        public static IReadOnlyList<Exception> UnwrapResultsOrExceptions<T>(
            this IEnumerable<ResultOrException<NoneResult>> source)
        {
            source.ThrowIfNull(nameof(source));

            return source
                .Where(item => !item.IsSuccess)
                .Select(item => item.Exception)
                .ToList();
        }

        public static (IReadOnlyList<TResult> taskResults, IReadOnlyList<Exception> taskExceptions)
            UnwrapResultsOrExceptions<TResult>(this IEnumerable<ResultOrException<TResult>> source)
        {
            source.ThrowIfNull(nameof(source));

            var taskResults = new List<TResult>();
            var taskExceptions = new List<Exception>();
            foreach (ResultOrException<TResult> item in source)
            {
                if (item.IsSuccess)
                    taskResults.Add(item.Result);
                else
                    taskExceptions.Add(item.Exception);
            }

            return (taskResults, taskExceptions);
        }

        public static Task<TResult> CancelIfFaulted<TResult>(
            this Task<TResult> task, CancellationTokenSource cancellationTokenSource)
        {
            return task.ContinueWith(
                task =>
                {
                    switch (task.Status)
                    {
                        case TaskStatus.RanToCompletion:
                            return task.Result;

                        case TaskStatus.Faulted:
                            cancellationTokenSource.Cancel();
                            return default!;

                        default:
                            return default!;
                    }
                }
            );
        }
    }
}
