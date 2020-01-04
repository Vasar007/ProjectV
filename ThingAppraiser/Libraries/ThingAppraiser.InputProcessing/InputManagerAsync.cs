using System;
using System.Collections.Generic;
using System.Linq;
using Acolyte.Assertions;
using ThingAppraiser.Communication;
using ThingAppraiser.DataPipeline;
using ThingAppraiser.Logging;

namespace ThingAppraiser.IO.Input
{
    public sealed class InputManagerAsync : IManager<IInputterAsync>
    {
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor<InputManagerAsync>();

        private readonly string _defaultStorageName;

        private readonly List<IInputterAsync> _inputtersAsync = new List<IInputterAsync>();


        public InputManagerAsync(string defaultStorageName)
        {
            _defaultStorageName =
                defaultStorageName.ThrowIfNullOrWhiteSpace(nameof(defaultStorageName));
        }

        #region IManager<IInputterAsync> Implementation

        public void Add(IInputterAsync item)
        {
            item.ThrowIfNull(nameof(item));
            if (!_inputtersAsync.Contains(item))
            {
                _inputtersAsync.Add(item);
            }
        }

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
