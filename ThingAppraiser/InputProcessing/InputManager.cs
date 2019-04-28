using System;
using System.Collections.Generic;
using ThingAppraiser.Logging;
using ThingAppraiser.Communication;

namespace ThingAppraiser.IO.Input
{
    /// <summary>
    /// Class to read The Things name from input.
    /// </summary>
    public sealed class CInputManager : IManager<IInputter>
    {
        /// <summary>
        /// Logger instance for current class.
        /// </summary>
        private static readonly CLoggerAbstraction s_logger =
            CLoggerAbstraction.CreateLoggerInstanceFor<CInputManager>();

        /// <summary>
        /// Default storage name if user will not specify it.
        /// </summary>
        private readonly String _defaultFilename;

        /// <summary>
        /// Collection of concrete inputter classes which can save results to specified source.
        /// </summary>
        private readonly List<IInputter> _inputters = new List<IInputter>();


        /// <summary>
        /// Initializes instance according to parameter values.
        /// </summary>
        /// <param name="defaultFilename">Default file name when user doesn't provide it.</param>
        /// <exception cref="ArgumentException">
        /// <param name="defaultFilename">defaultFilename</param> is <c>null</c> or presents empty
        /// string.
        /// </exception>
        public CInputManager(String defaultFilename)
        {
            _defaultFilename = defaultFilename.ThrowIfNullOrEmpty(nameof(defaultFilename));
        }

        #region IManager<IInputter> Implementation

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">
        /// <paramref name="item">item</paramref> is <c>null</c>.
        /// </exception>
        public void Add(IInputter item)
        {
            item.ThrowIfNull(nameof(item));
            if (!_inputters.Contains(item))
            {
                _inputters.Add(item);
            }
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">
        /// <paramref name="item">item</paramref> is <c>null</c>.
        /// </exception>
        public Boolean Remove(IInputter item)
        {
            item.ThrowIfNull(nameof(item));
            return _inputters.Remove(item);
        }

        #endregion

        /// <summary>
        /// Get names from inputter with specified storage name.
        /// </summary>
        /// <param name="storageName">Input storage name.</param>
        /// <returns>Collection of The Things names as strings.</returns>
        public List<String> GetNames(String storageName)
        {
            var result = new List<String>();
            if (String.IsNullOrEmpty(storageName))
            {
                storageName = _defaultFilename;
            }

            foreach (IInputter inputter in _inputters)
            {
                Boolean success = TryReadThingNames(inputter, storageName, out List<String> value);

                if (!success || value.IsNullOrEmpty())
                {
                    String message = $"No Things were found in {storageName} by inputter " +
                                     $"{inputter.Tag}.";

                    s_logger.Warn(message);
                    SGlobalMessageHandler.OutputMessage(message);
                    continue;
                }

                result.AddRange(value);
            }

            s_logger.Info($"{result.Count} Things were found.");
            return result;
        }

        /// <summary>
        /// Calls reading method from inputter and process caught exceptions.
        /// </summary>
        /// <param name="inputter">Reader of input source.</param>
        /// <param name="result">Reference to collection to write data.</param>
        /// <param name="storageName">Input storage name.</param>
        /// <returns>
        /// <c>true</c> if read method doesn't throw any exceptions, <c>false</c> otherwise.
        /// </returns>
        private Boolean TryReadThingNames(IInputter inputter, String storageName,
            out List<String> result)
        {
            try
            {
                result = inputter.ReadThingNames(storageName);
            }
            catch (Exception ex)
            {
                s_logger.Warn(ex, "Couldn't get access to the storage.");
                SGlobalMessageHandler.OutputMessage("Couldn't get access to the storage for " +
                                                    $"inputter {inputter.Tag}. " +
                                                    $"Error: {ex.Message}");
                result = new List<String>();
                return false;
            }
            return true;
        }
    }
}
