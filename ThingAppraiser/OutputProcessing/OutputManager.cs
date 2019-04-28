using System;
using System.Collections.Generic;
using System.Linq;
using ThingAppraiser.Logging;
using ThingAppraiser.Data;

namespace ThingAppraiser.IO.Output
{
    /// <summary>
    /// Class which controlling results saving.
    /// </summary>
    public sealed class COutputManager : IManager<IOutputter>
    {
        /// <summary>
        /// Logger instance for current class.
        /// </summary>
        private static readonly CLoggerAbstraction s_logger =
            CLoggerAbstraction.CreateLoggerInstanceFor<COutputManager>();

        /// <summary>
        /// Default storage name if user will not specify it.
        /// </summary>
        private readonly String _defaultFilename;

        /// <summary>
        /// Collection of concrete outputter classes which can save results to specified source.
        /// </summary>
        private readonly List<IOutputter> _outputters = new List<IOutputter>();


        /// <summary>
        /// Initializes instance according to parameter values.
        /// </summary>
        /// <param name="defaultFilename">Default file name when user doesn't provide it.</param>
        /// <exception cref="ArgumentException">
        /// <param name="defaultFilename">defaultFilename</param> is <c>null</c> or presents empty
        /// string.
        /// </exception>
        public COutputManager(String defaultFilename)
        {
            _defaultFilename = defaultFilename.ThrowIfNullOrEmpty(nameof(defaultFilename));
        }

        #region IManager<IOutputter> Implementation

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">
        /// <paramref name="item">item</paramref> is <c>null</c>.
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
        /// <paramref name="item">item</paramref> is <c>null</c>.
        /// </exception>
        public Boolean Remove(IOutputter item)
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
        public Boolean SaveResults(List<List<CRatingDataContainer>> results, String storageName)
        {
            if (String.IsNullOrEmpty(storageName))
            {
                storageName = _defaultFilename;
            }

            List<Boolean> statuses = _outputters.Select(
                outputter => outputter.SaveResults(results, storageName)
            ).ToList();

            if (!statuses.IsNullOrEmpty() && statuses.All(r => r))
            {
                s_logger.Info($"Successfully saved all results to \"{storageName}\".");
                return true;
            }

            s_logger.Info($"Couldn't save some results to \"{storageName}\".");
            return false;
        }
    }
}
