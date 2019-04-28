using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ThingAppraiser.Logging;

namespace ThingAppraiser.IO.Input
{
    public sealed class CInputManagerRx: IManager<IInputterRx>
    {
        private static readonly CLoggerAbstraction s_logger =
            CLoggerAbstraction.CreateLoggerInstanceFor<CInputManagerRx>();

        private readonly String _defaultFilename;

        private readonly List<IInputterRx> _inputtersRx = new List<IInputterRx>();


        public CInputManagerRx(String defaultFilename)
        {
            _defaultFilename = defaultFilename.ThrowIfNullOrEmpty(nameof(defaultFilename));
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

        public Boolean Remove(IInputterRx item)
        {
            item.ThrowIfNull(nameof(item));
            return _inputtersRx.Remove(item);
        }

        #endregion

        public IObservable<String> GetNames(String storageName)
        {
            if (String.IsNullOrEmpty(storageName))
            {
                storageName = _defaultFilename;
            }

            IObservable<String> inputQueue = _inputtersRx.AsParallel().Select(inputterRx => 
                Observable.Create((IObserver<String> observer) =>
                {
                    try
                    {
                        foreach (String value in inputterRx.ReadThingNames(storageName))
                        {
                            observer.OnNext(value);
                        }

                        observer.OnCompleted();
                    }
                    catch (Exception ex)
                    {
                        s_logger.Error(ex, $"Inputter {inputterRx.Tag} threw exception.");
                        observer.OnError(ex);
                    }

                    return Disposable.Empty;
                })
            ).Merge();

            s_logger.Info($"Inputters were configured to read from \"{storageName}\".");
            return inputQueue;
        }
    }
}
