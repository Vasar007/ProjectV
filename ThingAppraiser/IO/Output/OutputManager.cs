using System.Collections.Generic;

namespace ThingAppraiser.IO.Output
{
    /// <summary>
    /// Class which controlling results saving.
    /// </summary>
    public class OutputManager
    {
        #region Const & Static Fields

        /// <summary>
        /// Logger instance for current class.
        /// </summary>
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Default storage name if user will not specify it.
        /// </summary>
        private const string _defaultFilename = "apparaised_things.csv";

        #endregion

        #region Class Fields

        /// <summary>
        /// Implementation of outputter class which can save results to specified source.
        /// </summary>
        private IOutputter _outputter;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor with dependency injection.
        /// </summary>
        /// <param name="outputter">Instance of <see cref="IOutputter"/>.</param>
        public OutputManager(IOutputter outputter)
        {
            _outputter = outputter;
        }

        #endregion

        #region Public Class Methods

        /// <summary>
        /// Execute saving procedure and get it status as boolean variable.
        /// </summary>
        /// <param name="results">Collections of appraised results to save.</param>
        /// <param name="storageName">Storage name of output source.</param>
        /// <returns>True if the save was successful, false otherwise.</returns>
        public bool SaveResults(List<List<Data.ResultType>> results,
                                string storageName = _defaultFilename)
        {
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
