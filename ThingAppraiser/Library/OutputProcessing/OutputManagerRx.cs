using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ThingAppraiser.Data;
using ThingAppraiser.Logging;

namespace ThingAppraiser.IO.Output
{
    public sealed class OutputManagerRx : IManager<IOutputterRx>
    {
        private static readonly LoggerAbstraction _logger =
            LoggerAbstraction.CreateLoggerInstanceFor<OutputManagerRx>();

        private readonly string _defaultStorageName;

        private readonly List<IOutputterRx> _outputtersRx = new List<IOutputterRx>();


        public OutputManagerRx(string defaultStorageName)
        {
            _defaultStorageName = defaultStorageName.ThrowIfNullOrWhiteSpace(
                nameof(defaultStorageName)
            );
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

        public bool Remove(IOutputterRx item)
        {
            item.ThrowIfNull(nameof(item));
            return _outputtersRx.Remove(item);
        }

        #endregion
        
        public async Task<bool> SaveResults(
            IList<IObservable<RatingDataContainer>> appraisedDataQueues, string storageName)
        {
            if (string.IsNullOrWhiteSpace(storageName))
            {
                storageName = _defaultStorageName;

                _logger.Info("Storage name is empty, using the default value.");
            }

            List<bool> statuses = await Consume(appraisedDataQueues, storageName);

            if (!statuses.IsNullOrEmpty() && statuses.All(r => r))
            {
                _logger.Info($"Successfully saved all results to \"{storageName}\".");
                return true;
            }

            _logger.Info($"Couldn't save some results to \"{storageName}\".");
            return false;
        }

        private async Task<List<bool>> Consume(
            IList<IObservable<RatingDataContainer>> appraisedDataQueues, string storageName)
        {
            var consumed = new ConcurrentBag<ConcurrentBag<RatingDataContainer>>();
            var consumers = new List<Task>();

            foreach (IObservable<RatingDataContainer> resultQueue  in appraisedDataQueues)
            {
                var rating = new ConcurrentBag<RatingDataContainer>();
                consumed.Add(rating);

                Task task = resultQueue
                    .ObserveOn(ThreadPoolScheduler.Instance)
                    .ForEachAsync(dataContainer => rating.Add(dataContainer));

                consumers.Add(task);
            }

            await Task.WhenAll(consumers);

            List<List<RatingDataContainer>> results = consumed
                .Select(x => x.ToList())
                .ToList();

            results
                .AsParallel()
                .ForAll(rating => rating.Sort((x, y) => y.RatingValue.CompareTo(x.RatingValue)));

            List<bool> outputterStatuses = _outputtersRx
                .AsParallel()
                .Select(outputterRx => outputterRx.SaveResults(results, storageName))
                .ToList();

            _logger.Info("Outputters were configured.");

            return outputterStatuses;
        }
    }
}
