using System;
using NLog;

namespace ThingAppraiser.Logging
{
    /// <summary>
    /// Additional abstraction to avoid direct link with logger library.
    /// </summary>
    public class LoggerAbstraction
    {
        /// <summary>
        /// Concrete logger instance.
        /// </summary>
        /// <remarks>Try to use logger as private static readonly field in code.</remarks>
        private readonly Logger _logger;


        /// <summary>
        /// Private constructor which could be called by create methods.
        /// </summary>
        /// <param name="loggerInstance">Concrete logger instance.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="loggerInstance" /> is <c>null</c>.
        /// </exception>
        private LoggerAbstraction(Logger loggerInstance)
        {
            _logger = loggerInstance.ThrowIfNull(nameof(loggerInstance));
        }

        /// <summary>
        /// Creates logger instance for passed type.
        /// </summary>
        /// <typeparam name="T">Type for which instance is created.</typeparam>
        /// <returns>Created logger instance.</returns>
        /// <exception cref="ArgumentException">
        /// Cannot get full name of type <see cref="T" />
        /// </exception>
        public static LoggerAbstraction CreateLoggerInstanceFor<T>()
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
        public static LoggerAbstraction CreateLoggerInstanceWithName(string className)
        {
            className.ThrowIfNullOrEmpty(nameof(className));
            return new LoggerAbstraction(LogManager.GetLogger(className));
        }

        /// <inheritdoc cref="Logger.Debug(string)" />
        public void Debug(string message)
        {
            _logger.Debug(message);
        }

        /// <inheritdoc cref="Logger.Info(string)" />
        public void Info(string message)
        {
            _logger.Info(message);
        }

        /// <inheritdoc cref="Logger.Warn(string)" />
        public void Warn(string message)
        {
            _logger.Warn(message);
        }

        /// <inheritdoc cref="Logger.Warn(Exception, string)" />
        public void Warn(Exception ex, string message)
        {
            _logger.Warn(ex, message);
        }

        /// <inheritdoc cref="Logger.Error(string)" />
        public void Error(string message)
        {
            _logger.Error(message);
        }

        /// <inheritdoc cref="Logger.Error(Exception, string)" />
        public void Error(Exception ex, string message)
        {
            _logger.Error(ex, message);
        }
    }
}
