using System;
using System.Collections.Generic;
using ThingAppraiser.Logging;
using ThingAppraiser.Communication;

namespace ThingAppraiser.IO.Input
{
    /// <summary>
    /// Class to read The Things name from input.
    /// </summary>
    public sealed class InputManager : IManager<IInputter>
    {
        /// <summary>
        /// Logger instance for current class.
        /// </summary>
        private static readonly LoggerAbstraction _logger =
            LoggerAbstraction.CreateLoggerInstanceFor<InputManager>();

        /// <summary>
        /// Default storage name if user will not specify it.
        /// </summary>
        private readonly string _defaultStorageName;

        /// <summary>
        /// Collection of concrete inputter classes which can save results to specified source.
        /// </summary>
        private readonly List<IInputter> _inputters = new List<IInputter>();


        /// <summary>
        /// Initializes instance according to parameter values.
        /// </summary>
        /// <param name="defaultStorageName">Default file name when user doesn't provide it.</param>
        /// <exception cref="ArgumentException">
        /// <param name="defaultStorageName">defaultStorageName</param> is <c>null</c> or presents 
        /// empty string.
        /// </exception>
        public InputManager(string defaultStorageName)
        {
            _defaultStorageName = defaultStorageName.ThrowIfNullOrWhiteSpace(
                nameof(defaultStorageName)
            );
        }

        #region IManager<IInputter> Implementation

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">
        /// <paramref name="item" /> is <c>null</c>.
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
        /// <paramref name="item" /> is <c>null</c>.
        /// </exception>
        public bool Remove(IInputter item)
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
        public List<string> GetNames(string storageName)
        {
            var result = new List<string>();
            if (string.IsNullOrWhiteSpace(storageName))
            {
                storageName = _defaultStorageName;

                string message = "Storage name is empty, using the default value.";
                _logger.Info(message);
                GlobalMessageHandler.OutputMessage(message);
            }

            foreach (IInputter inputter in _inputters)
            {
                bool success = TryReadThingNames(inputter, storageName, out List<string> value);

                if (!success || value.IsNullOrEmpty())
                {
                    string message = $"No Things were found in {storageName} by inputter " +
                                     $"{inputter.Tag}.";

                    _logger.Warn(message);
                    GlobalMessageHandler.OutputMessage(message);
                    continue;
                }

                result.AddRange(value);
            }

            _logger.Info($"{result.Count} Things were found.");
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
        private bool TryReadThingNames(IInputter inputter, string storageName,
            out List<string> result)
        {
            try
            {
                result = inputter.ReadThingNames(storageName);
            }
            catch (Exception ex)
            {
                _logger.Warn(ex, "Couldn't get access to the storage.");
                GlobalMessageHandler.OutputMessage("Couldn't get access to the storage for " +
                                                    $"inputter {inputter.Tag}. Error: {ex}");
                result = new List<string>();
                return false;
            }
            return true;
        }
    }
}
