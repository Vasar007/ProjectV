using System;
using NLog;

namespace ThingAppraiser.Logging
{
    /// <summary>
    /// Additional abstraction to avoid direct link with logger library.
    /// </summary>
    public class CLoggerAbstraction
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
        /// <paramref name="loggerInstance">loggerInstance</paramref> is <c>null</c>.
        /// </exception>
        private CLoggerAbstraction(Logger loggerInstance)
        {
            _logger = loggerInstance.ThrowIfNull(nameof(loggerInstance));
        }

        /// <summary>
        /// Creates logger instance for passed type.
        /// </summary>
        /// <typeparam name="T">Type for which instance is created.</typeparam>
        /// <returns>Created logger instance.</returns>
        /// <exception cref="ArgumentException">
        /// Cannot get full name of type <see cref="T"/>
        /// </exception>
        public static CLoggerAbstraction CreateLoggerInstanceFor<T>()
        {
            String fullName = typeof(T).FullName ?? throw new ArgumentException(
                $"Could not get full name of class {nameof(T)}"
            );
            return new CLoggerAbstraction(LogManager.GetLogger(fullName));
        }

        /// <summary>
        /// Creates logger instance for passed class name.
        /// </summary>
        /// <param name="className">Class name. Try to pass it with nameof operator.</param>
        /// <returns>Created logger instance.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="className">loggerInstance</paramref> is <c>null</c> or presents empty
        /// string.
        /// </exception>
        public static CLoggerAbstraction CreateLoggerInstanceWithName(String className)
        {
            className.ThrowIfNullOrEmpty(nameof(className));
            return new CLoggerAbstraction(LogManager.GetLogger(className));
        }

        /// <inheritdoc cref="Logger.Debug(String)" />
        public void Debug(String message)
        {
            _logger.Debug(message);
        }

        /// <inheritdoc cref="Logger.Info(String)" />
        public void Info(String message)
        {
            _logger.Info(message);
        }

        /// <inheritdoc cref="Logger.Warn(String)" />
        public void Warn(String message)
        {
            _logger.Warn(message);
        }

        /// <inheritdoc cref="Logger.Warn(Exception, String)" />
        public void Warn(Exception ex, String message)
        {
            _logger.Warn(ex, message);
        }

        /// <inheritdoc cref="Logger.Error(String)" />
        public void Error(String message)
        {
            _logger.Error(message);
        }

        /// <inheritdoc cref="Logger.Error(Exception, String)" />
        public void Error(Exception ex, String message)
        {
            _logger.Error(ex, message);
        }
    }
}
