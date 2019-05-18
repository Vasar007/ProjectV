using System;
using System.Collections.Generic;
using System.Linq;
using ThingAppraiser.Logging;
using ThingAppraiser.Data;
using ThingAppraiser.Communication;

namespace ThingAppraiser.IO.Output
{
    /// <summary>
    /// Class which controlling results saving.
    /// </summary>
    public sealed class OutputManager : IManager<IOutputter>
    {
        /// <summary>
        /// Logger instance for current class.
        /// </summary>
        private static readonly LoggerAbstraction _logger =
            LoggerAbstraction.CreateLoggerInstanceFor<OutputManager>();

        /// <summary>
        /// Default storage name if user will not specify it.
        /// </summary>
        private readonly string _defaultStorageName;

        /// <summary>
        /// Collection of concrete outputter classes which can save results to specified source.
        /// </summary>
        private readonly List<IOutputter> _outputters = new List<IOutputter>();


        /// <summary>
        /// Initializes instance according to parameter values.
        /// </summary>
        /// <param name="defaultStorageName">Default file name when user doesn't provide it.</param>
        /// <exception cref="ArgumentException">
        /// <param name="defaultStorageName">defaultStorageName</param> is <c>null</c>, presents 
        /// empty strings or contains only whitespaces.
        /// </exception>
        public OutputManager(string defaultStorageName)
        {
            _defaultStorageName = defaultStorageName.ThrowIfNullOrWhiteSpace(
                nameof(defaultStorageName)
            );
        }

        #region IManager<IOutputter> Implementation

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">
        /// <paramref name="item" /> is <c>null</c>.
        /// </exception>
        public void Add(IOutputter item)
        {
            item.ThrowIfNull(nameof(item));
            if (!_outputters.Contains(item))
            {
                _outputters.Add(item);
            }
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">
        /// <paramref name="item" /> is <c>null</c>.
        /// </exception>
        public bool Remove(IOutputter item)
        {
            item.ThrowIfNull(nameof(item));
            return _outputters.Remove(item);
        }

        #endregion

        /// <summary>
        /// Executes saving procedure and get it status as boolean variable.
        /// </summary>
        /// <param name="results">Collections of appraised results to save.</param>
        /// <param name="storageName">Storage name of output source.</param>
        /// <returns><c>true</c> if the save was successful, <c>false</c> otherwise.</returns>
        public bool SaveResults(List<List<RatingDataContainer>> results, string storageName)
        {
            if (string.IsNullOrWhiteSpace(storageName))
            {
                storageName = _defaultStorageName;

                string message = "Storage name is empty, using the default value.";
                _logger.Info(message);
                GlobalMessageHandler.OutputMessage(message);
            }

            List<bool> statuses = _outputters.Select(
                outputter => outputter.SaveResults(results, storageName)
            ).ToList();

            if (!statuses.IsNullOrEmpty() && statuses.All(r => r))
            {
                _logger.Info($"Successfully saved all results to \"{storageName}\".");
                return true;
            }

            _logger.Info($"Couldn't save some results to \"{storageName}\".");
            return false;
        }
    }
}
