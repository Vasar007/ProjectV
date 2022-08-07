using System;
using System.Collections.Generic;
using Acolyte.Linq;

namespace ProjectV.Logging
{
    public static class LoggerExtensions
    {
        public static void Warns(this ILogger logger, IReadOnlyList<Exception> exceptions,
            string message)
        {
            ProcessExceptionsInternal(exceptions, ex => logger.Warn(ex, message));
        }

        public static void Errors(this ILogger logger, IReadOnlyList<Exception> exceptions,
            string message)
        {
            ProcessExceptionsInternal(exceptions, ex => logger.Error(ex, message));
        }

        private static void ProcessExceptionsInternal(IReadOnlyList<Exception> exceptions,
            Action<Exception> action)
        {
            exceptions.ForEach(action);
        }
    }
}
