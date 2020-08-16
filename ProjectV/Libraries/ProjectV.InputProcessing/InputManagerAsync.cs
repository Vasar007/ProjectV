using System;
using System.Collections.Generic;
using System.Linq;
using Acolyte.Assertions;
using ProjectV.Communication;
using ProjectV.DataPipeline;
using ProjectV.Logging;

namespace ProjectV.IO.Input
{
    /// <summary>
    /// Class to read The Things name from input.
    /// </summary>
    public sealed class InputManagerAsync : IManager<IInputterAsync>
    {
        /// <summary>
        /// Logger instance for current class.
        /// </summary>
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor<InputManagerAsync>();

        /// <summary>
        /// Default storage name if user will not specify it.
        /// </summary>
        private readonly string _defaultStorageName;

        /// <summary>
        /// Collection of concrete inputter classes which can save results to specified source.
        /// </summary>
        private readonly List<IInputterAsync> _inputtersAsync = new List<IInputterAsync>();


        /// <summary>
        /// Initializes instance according to parameter values.
        /// </summary>
        /// <param name="defaultStorageName">Default file name when user doesn't provide it.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="defaultStorageName" /> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="defaultStorageName" /> presents empty strings or contains only
        /// whitespaces.
        /// </exception>
        public InputManagerAsync(string defaultStorageName)
        {
            _defaultStorageName =
                defaultStorageName.ThrowIfNullOrWhiteSpace(nameof(defaultStorageName));
        }

        #region IManager<IInputterAsync> Implementation

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">
        /// <paramref name="item" /> is <c>null</c>.
        /// </exception>
        public void Add(IInputterAsync item)
        {
            item.ThrowIfNull(nameof(item));
            if (!_inputtersAsync.Contains(item))
            {
                _inputtersAsync.Add(item);
            }
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">
        /// <paramref name="item" /> is <c>null</c>.
        /// </exception>
        public bool Remove(IInputterAsync item)
        {
            item.ThrowIfNull(nameof(item));
            return _inputtersAsync.Remove(item);
        }

        #endregion

        public InputtersFlow CreateFlow(string storageName)
        {
            if (string.IsNullOrWhiteSpace(storageName))
            {
                storageName = _defaultStorageName;

                const string message = "Storage name is empty, using the default value.";
                _logger.Info(message);
                GlobalMessageHandler.OutputMessage(message);
            }

            var inputtersFunc = _inputtersAsync.Select(inputterAsync =>
                new Func<string, IEnumerable<string>>(
                    input => TryReadThingNames(inputterAsync, input)
                )
            );

            var inputtersFlow = new InputtersFlow(inputtersFunc);

            _logger.Info($"Conctructed inputters pipeline for \"{storageName}\".");
            return inputtersFlow;
        }

        /// <summary>
        /// Get names from inputter with specified storage name.
        /// </summary>
        /// <param name="storageName">Input storage name.</param>
        /// <returns>Enumeration of The Things names as strings.</returns>
        private static IEnumerable<string> TryReadThingNames(IInputterAsync inputterAsync,
            string storageName)
        {
            try
            {
                return inputterAsync.ReadThingNames(storageName);
            }
            catch (Exception ex)
            {
                string message = $"Inputter '{inputterAsync.Tag}' could not get access to the " +
                                 $"storage \"{storageName}\".";
                _logger.Error(ex, message);
                throw;
            }
        }
    }
}
