using System;
using NLog;

namespace ThingAppraiser.Logging
{
    public static class LoggerFactory
    {
        /// <summary>
        /// Creates logger instance for passed type.
        /// </summary>
        /// <typeparam name="T">Type for which instance is created.</typeparam>
        /// <returns>Created logger instance.</returns>
        /// <exception cref="ArgumentException">
        /// Cannot get full name of type <see cref="T" />
        /// </exception>
        public static ILogger CreateLoggerFor<T>()
        {
            string fullName = typeof(T).FullName ?? throw new ArgumentException(
                $"Could not get full name of class {nameof(T)}"
            );
            return new LoggerAbstraction(LogManager.GetLogger(fullName));
        }

        /// <summary>
        /// Creates logger instance for passed class name.
        /// </summary>
        /// <param name="className">Class name. Try to pass it with nameof operator.</param>
        /// <returns>Created logger instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="className" /> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="className" /> presents empty string.
        /// </exception>
        public static ILogger CreateLoggerWithName(string className)
        {
            className.ThrowIfNullOrEmpty(nameof(className));

            return new LoggerAbstraction(LogManager.GetLogger(className));
        }
    }
}
