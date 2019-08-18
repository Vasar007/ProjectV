using System;
using NLog;

namespace ThingAppraiser.Logging
{
    /// <summary>
    /// Additional abstraction to avoid direct link with logger library.
    /// </summary>
    internal sealed class NLogLoggerAbstraction : ILogger
    {
        /// <summary>
        /// Concrete logger instance.
        /// </summary>
        /// <remarks>Try to use logger as private static readonly field in code.</remarks>
        private readonly Logger _logger;


        /// <summary>
        /// Private constructor which could be called by create methods.
        /// </summary>
        /// <param name="loggerName">Logger name to create.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="loggerName" /> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="loggerName" /> presents empty string.
        /// </exception>
        internal NLogLoggerAbstraction(string loggerName)
        {
            loggerName.ThrowIfNullOrEmpty(nameof(loggerName));

            _logger = LogManager.GetLogger(loggerName);
        }

        #region ILogger Implementation

        /// <summary>
        /// Prints header info.
        /// </summary>
        /// <param name="message">Additional message to print.</param>
        public void PrintHeader(string message)
        {
            TimeSpan offset = TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow);
            _logger.Info($"UTC offset is {offset}.");

            _logger.Info(message);
        }

        /// <summary>
        /// Prints footer info.
        /// </summary>
        /// <param name="message">Additional message to print.</param>
        public void PrintFooter(string message)
        {
            _logger.Info(message);
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

        #endregion
    }
}
