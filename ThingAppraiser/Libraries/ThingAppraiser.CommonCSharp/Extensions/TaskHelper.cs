using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ThingAppraiser.Extensions
{
    public static class TaskHelper
    {
        public static Task<ResultOrException<TResult>[]> WhenAllResultsOrExceptions<TResult>(
            params Task<TResult>[] tasks)
        {
            return Task.WhenAll(tasks.Select(task => task.WrapResultOrExceptionAsync()));
        }

        public static Task<ResultOrException<TResult>[]> WhenAllResultsOrExceptions<TResult>(
            IEnumerable<Task<TResult>> tasks)
        {
            return Task.WhenAll(tasks.Select(task => task.WrapResultOrExceptionAsync()));
        }

        public static Task<ResultOrException<NoneResult>[]> WhenAllResultsOrExceptions(
            params Task[] tasks)
        {
            return Task.WhenAll(tasks.Select(task => task.WrapResultOrExceptionAsync()));
        }

        public static Task<ResultOrException<NoneResult>[]> WhenAllResultsOrExceptions(
            IEnumerable<Task> tasks)
        {
            return Task.WhenAll(tasks.Select(task => task.WrapResultOrExceptionAsync()));
        }

        public static Task WhenAllTasks(params Task[] tasks)
        {
            return WhenAllTasks(tasks);
        }

        public static Task WhenAllTasks(IEnumerable<Task> tasks)
        {
            tasks.ThrowIfNull(nameof(tasks));

            Task allTasks = Task.WhenAll(tasks);
            try
            {
                allTasks.GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Trace.TraceError($"{nameof(WhenAllTasks)} Exception: {ex.ToString()}");
            }

            Trace.TraceInformation($"{nameof(WhenAllTasks)} status: {allTasks.Status}");

            if (allTasks.Exception != null)
            {
                throw allTasks.Exception;
            }

            return allTasks;
        }

        public static Task<TResult[]> WhenAllTasks<TResult>(params Task<TResult>[] tasks)
        {
            return WhenAllTasks(tasks);
        }

        public static Task<TResult[]> WhenAllTasks<TResult>(IEnumerable<Task<TResult>> tasks)
        {
            tasks.ThrowIfNull(nameof(tasks));

            Task<TResult[]> allTasks = Task.WhenAll(tasks);
            try
            {
                allTasks.GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Trace.TraceError($"{nameof(WhenAllTasks)} Exception: {ex.ToString()}");
            }

            Trace.TraceInformation($"{nameof(WhenAllTasks)} status: {allTasks.Status.ToString()}.");

            if (allTasks.Exception != null)
            {
                throw allTasks.Exception;
            }

            return allTasks;
        }
    }
}
