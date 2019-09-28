using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ThingAppraiser.Extensions
{
    public static class TaskExtensions
    {
        public static async Task<ResultOrException<T>> WrapResultOrExceptionAsync<T>(
            this Task<T> task)
        {
            try
            {
                T taskResult = await task;
                return new ResultOrException<T>(taskResult);
            }
            catch (Exception ex)
            {
                return new ResultOrException<T>(ex);
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
            return source
                .Where(item => !item.IsSuccess)
                .Select(item => item.Exception)
                .ToList();
        }

        public static (IReadOnlyList<T> taskResults, IReadOnlyList<Exception> taskExceptions)
            UnwrapResultsOrExceptions<T>(this IEnumerable<ResultOrException<T>> source)
        {
            var taskResults = new List<T>();
            var taskExceptions = new List<Exception>();
            foreach (ResultOrException<T> item in source)
            {
                if (item.IsSuccess)
                    taskResults.Add(item.Result);
                else
                    taskExceptions.Add(item.Exception);
            }

            return (taskResults, taskExceptions);
        }
    }
}
