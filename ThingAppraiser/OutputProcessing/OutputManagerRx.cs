using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ThingAppraiser.Data;
using ThingAppraiser.Logging;

namespace ThingAppraiser.IO.Output
{
    public sealed class COutputManagerRx : IManager<IOutputterRx>
    {
        private static readonly CLoggerAbstraction s_logger =
            CLoggerAbstraction.CreateLoggerInstanceFor<COutputManagerRx>();

        private readonly String _defaultFilename;

        private readonly List<IOutputterRx> _outputtersRx = new List<IOutputterRx>();


        public COutputManagerRx(String defaultFilename)
        {
            _defaultFilename = defaultFilename.ThrowIfNullOrEmpty(nameof(defaultFilename));
        }

        #region IManager<IOutputterRx> Implementation

        public void Add(IOutputterRx item)
        {
            item.ThrowIfNull(nameof(item));
            if (!_outputtersRx.Contains(item))
            {
                _outputtersRx.Add(item);
            }
        }

        public Boolean Remove(IOutputterRx item)
        {
            item.ThrowIfNull(nameof(item));
            return _outputtersRx.Remove(item);
        }

        #endregion
        
        public async Task<Boolean> SaveResults(IObservable<CRatingDataContainer> resultsQueues,
            String storageName)
        {
            if (String.IsNullOrEmpty(storageName))
            {
                storageName = _defaultFilename;
            }

            List<Boolean> statuses = await Consume(resultsQueues, storageName);

            if (!statuses.IsNullOrEmpty() && statuses.All(r => r))
            {
                s_logger.Info($"Successfully saved all results to \"{storageName}\".");
                return true;
            }

            s_logger.Info($"Couldn't save some results to \"{storageName}\".");
            return false;
        }

        private Task<List<Boolean>> Consume(IObservable<CRatingDataContainer> resultsQueue,
            String storageName)
        {
            var converted = new ConcurrentDictionary<String, ConcurrentBag<Double>>();

            Task drainTask = resultsQueue.ObserveOn(ThreadPoolScheduler.Instance).ForEachAsync(
                dataContainer =>
                {
                    if (converted.TryGetValue(dataContainer.DataHandler.Title,
                                              out ConcurrentBag<Double> ratingValues))
                    {
                        ratingValues.Add(dataContainer.RatingValue);
                    }
                    else
                    {
                        converted.TryAdd(dataContainer.DataHandler.Title,
                                         new ConcurrentBag<Double> { dataContainer.RatingValue });
                    }
                }
            );

            Task<List<Boolean>> continueWithTask = drainTask.ContinueWith(task =>
            {
                IReadOnlyList<COuputFileData> outputData = converted
                    .Select(
                        result => new COuputFileData
                        {
                            thingName = $"\"{result.Key}\"", // Escape Thing names.
                            ratingValue = result.Value.ToList()
                        })
                    .OrderByDescending(x => x.ratingValue.First())
                    .ToList();

                List<Boolean> outputterStatuses = _outputtersRx.AsParallel().Select(
                    outputterRx => outputterRx.SaveResults(outputData, storageName)
                ).ToList();

                s_logger.Info("Outputters were configured.");

                return outputterStatuses;
            });

            return continueWithTask;
        }
    }
}
