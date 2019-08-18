using System;
using NLog;

namespace ThingAppraiser.Logging
{
    /// <summary>
    /// Additional abstraction to avoid direct link with logger library.
    /// </summary>
    internal sealed class LoggerAbstraction : ILogger
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
        internal LoggerAbstraction(Logger loggerInstance)
        {
            _logger = loggerInstance.ThrowIfNull(nameof(loggerInstance));
        }

        #region ILogger Implementation

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
