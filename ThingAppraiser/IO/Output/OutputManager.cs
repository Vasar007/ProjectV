using System.Collections.Generic;

namespace ThingAppraiser.IO.Output
{
    /// <summary>
    /// Class which controlling results saving.
    /// </summary>
    public class OutputManager
    {
        #region Static Fields

        /// <summary>
        /// Logger instance for current class.
        /// </summary>
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        #region Class Fields

        /// <summary>
        /// Default storage name if user will not specify it.
        /// </summary>
        private readonly string _defaultFilename;

        /// <summary>
        /// Implementation of outputter class which can save results to specified source.
        /// </summary>
        private IOutputter _outputter;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes instance according to parameter values.
        /// </summary>
        /// <param name="outputter">Instance of <see cref="IOutputter"/>.</param>
        public OutputManager(IOutputter outputter, string defaultFilename = "apparaised_things.csv")
        {
            _outputter = outputter;
            _defaultFilename = defaultFilename;
        }

        #endregion

        #region Public Class Methods

        /// <summary>
        /// Execute saving procedure and get it status as boolean variable.
        /// </summary>
        /// <param name="results">Collections of appraised results to save.</param>
        /// <param name="storageName">Storage name of output source.</param>
        /// <returns>True if the save was successful, false otherwise.</returns>
        public bool SaveResults(List<List<Data.ResultType>> results, string storageName = null)
        {
            if (string.IsNullOrEmpty(storageName))
            {
                storageName = _defaultFilename;
            }

            bool result = _outputter.SaveResults(results, storageName);
            if (result)
            {
                _logger.Info($"Successfully saved results to \"{storageName}\".");
            }
            else
            {
                _logger.Info($"Couldn't save results to \"{storageName}\".");
            }
            return result;
        }

        #endregion
    }
}
