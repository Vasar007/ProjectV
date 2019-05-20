using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using ThingAppraiser.Data;
using ThingAppraiser.Logging;

namespace ThingAppraiser.IO.Output
{
    public sealed class OutputManagerAsync : IManager<IOutputterAsync>
    {
        private static readonly LoggerAbstraction _logger =
            LoggerAbstraction.CreateLoggerInstanceFor<OutputManagerAsync>();

        private readonly string _defaultStorageName;

        private readonly ExecutionDataflowBlockOptions _consumerOptions;

        private readonly DataflowLinkOptions _linkOptions;

        private readonly List<IOutputterAsync> _outputtersAsync = new List<IOutputterAsync>();


        public OutputManagerAsync(string defaultStorageName)
        {
            _defaultStorageName = defaultStorageName.ThrowIfNullOrWhiteSpace(
                nameof(defaultStorageName)
            );

            _consumerOptions = new ExecutionDataflowBlockOptions { BoundedCapacity = 1 };
            _linkOptions = new DataflowLinkOptions { PropagateCompletion = true };
        }

        #region IManager<IOutputterAsync> Implementation

        public void Add(IOutputterAsync item)
        {
            item.ThrowIfNull(nameof(item));
            if (!_outputtersAsync.Contains(item))
            {
                _outputtersAsync.Add(item);
            }
        }

        public bool Remove(IOutputterAsync item)
        {
            item.ThrowIfNull(nameof(item));
            return _outputtersAsync.Remove(item);
        }

        #endregion

        public async Task<bool> SaveResults(
            IDictionary<Type, BufferBlock<RatingDataContainer>> resultsQueues, string storageName)
        {
            if (string.IsNullOrWhiteSpace(storageName))
            {
                storageName = _defaultStorageName;

                _logger.Info("Storage name is empty, using the default value.");
            }

            var consumers = new List<ActionBlock<RatingDataContainer>>();
            var results = new List<List<RatingDataContainer>>();
            foreach (KeyValuePair<Type, BufferBlock<RatingDataContainer>> keyValue in resultsQueues)
            {
                var rating = new List<RatingDataContainer>();
                results.Add(rating);

                var consumer = new ActionBlock<RatingDataContainer>(x => rating.Add(x),
                                                                     _consumerOptions);
                consumers.Add(consumer);
                keyValue.Value.LinkTo(consumer, _linkOptions);
            }

            await Task.WhenAll(consumers.Select(consumer => consumer.Completion));

            results.AsParallel().ForAll(
                rating => rating.Sort((x, y) => y.RatingValue.CompareTo(x.RatingValue))
            );

            List<Task<bool>> resultTasks = _outputtersAsync.Select(
                outputterAsync => outputterAsync.SaveResults(results, storageName)
            ).ToList();

            bool[] statuses = await Task.WhenAll(resultTasks);
            if (!statuses.IsNullOrEmpty() && statuses.All(r => r))
            {
                _logger.Info($"Successfully saved all results to \"{storageName}\".");
                return true;
            }

            _logger.Info($"Couldn't save some results to \"{storageName}\".");
            return false;
        }
    }
}
