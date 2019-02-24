using System;
using System.Collections.Generic;

namespace ThingAppraiser.IO.Input
{
    /// <summary>
    /// Class to read The Things name from input.
    /// </summary>
    public class InputManager
    {
        #region Static Fields

        /// <summary>
        /// Logger instance for current class.
        /// </summary>
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        #region Class Fields

        /// <summary>
        /// Number of attempts to read data from input.
        /// </summary>
        private readonly int _limitAttempts;

        /// <summary>
        /// Default storage name if user will not specify it.
        /// </summary>
        private readonly string _defaultFilename;

        /// <summary>
        /// Implementation of inputter class which can read The Things names from specified source.
        /// </summary>
        private IInputter _inputter;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes instance according to parameter values.
        /// </summary>
        /// <param name="inputter">Instance of <see cref="IInputter"/>.</param>
        /// <param name="defaultFilename">Default file name when user doesn't provide it.</param>
        /// <param name="limitAttempts">Limit of attempts to read from input.</param>
        public InputManager(IInputter inputter, string defaultFilename = "thing_names.txt",
            int limitAttempts = 10)
        {
            _inputter = inputter;
            _defaultFilename = defaultFilename;
            _limitAttempts = limitAttempts;
        }

        #endregion

        #region Private Class Methods

        /// <summary>
        /// Call read method from inputter and process caught exceptions.
        /// </summary>
        /// <param name="result">Reference to collection to write data.</param>
        /// <param name="storageName">Input storage name.</param>
        /// <returns>True if read method doesn't throw any exceptions, false otherwise.</returns>
        private bool TryReadThingNames(ref List<string> result, string storageName)
        {
            try
            {
                result = _inputter.ReadThingNames(storageName);
            }
            catch (Exception ex)
            {
                _logger.Warn(ex, "Couldn't get access to the storage.");
                Core.Shell.OutputMessage("Couldn't get access to the storage. " +
                                         $"Error: {ex.Message}");
                return false;
            }
            return true;
        }

        #endregion

        #region Public Class Methods

        /// <summary>
        /// Get names from inputter with specified storage name.
        /// </summary>
        /// <remarks>
        /// This method try to read data several attempts. Number of attempts is specified in the 
        /// configuration file.
        /// </remarks>
        /// <param name="storageName">Input storage name.</param>
        /// <returns>Collection of The Things names as strings.</returns>
        public List<string> GetNames(string storageName)
        {
            var result = new List<string>();
            if (!string.IsNullOrEmpty(storageName))
            {
                TryReadThingNames(ref result, storageName);
            }
            else
            {
                TryReadThingNames(ref result, _defaultFilename);
            }

            int numberOfAttempts = 1;
            while (result.Count == 0 && numberOfAttempts < _limitAttempts)
            {
                _logger.Warn("No Things were found.");
                Core.Shell.OutputMessage("No Things were found. Enter other storage name: ");
                TryReadThingNames(ref result, Core.Shell.GetMessage());
                ++numberOfAttempts;
            }

            _logger.Info($"{result.Count} Things were found.");
            return result;
        }

        #endregion
    }
}
