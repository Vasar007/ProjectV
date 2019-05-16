using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ThingAppraiser.Communication;
using ThingAppraiser.Logging;

namespace ThingAppraiser.IO.Input
{
    public sealed class InputManagerRx: IManager<IInputterRx>
    {
        private static readonly LoggerAbstraction _logger =
            LoggerAbstraction.CreateLoggerInstanceFor<InputManagerRx>();

        private readonly string _defaultStorageName;

        private readonly List<IInputterRx> _inputtersRx = new List<IInputterRx>();


        public InputManagerRx(string defaultStorageName)
        {
            _defaultStorageName = defaultStorageName.ThrowIfNullOrWhiteSpace(
                nameof(defaultStorageName)
            );
        }

        #region IManager<IInputterRx> Implementation

        public void Add(IInputterRx item)
        {
            item.ThrowIfNull(nameof(item));
            if (!_inputtersRx.Contains(item))
            {
                _inputtersRx.Add(item);
            }
        }

        public bool Remove(IInputterRx item)
        {
            item.ThrowIfNull(nameof(item));
            return _inputtersRx.Remove(item);
        }

        #endregion

        public IObservable<string> GetNames(string storageName)
        {
            if (string.IsNullOrWhiteSpace(storageName))
            {
                storageName = _defaultStorageName;

                string message = "Storage name is empty, using the default value.";
                _logger.Info(message);
                GlobalMessageHandler.OutputMessage(message);
            }

            IObservable<string> inputQueue = _inputtersRx.AsParallel().Select(inputterRx => 
                Observable.Create((IObserver<string> observer) =>
                {
                    try
                    {
                        foreach (string value in inputterRx.ReadThingNames(storageName))
                        {
                            observer.OnNext(value);
                        }

                        observer.OnCompleted();
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, $"Inputter {inputterRx.Tag} threw exception.");
                        observer.OnError(ex);
                    }

                    return Disposable.Empty;
                })
            ).Merge();

            _logger.Info($"Inputters were configured to read from \"{storageName}\".");
            return inputQueue;
        }
    }
}
