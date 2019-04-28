using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using ThingAppraiser.Data;
using ThingAppraiser.Logging;

namespace ThingAppraiser.IO.Output
{
    public sealed class COutputManagerAsync : IManager<IOutputterAsync>
    {
        private static readonly CLoggerAbstraction s_logger =
            CLoggerAbstraction.CreateLoggerInstanceFor<COutputManagerAsync>();

        private readonly String _defaultFilename;

        private readonly ExecutionDataflowBlockOptions _consumerOptions;

        private readonly DataflowLinkOptions _linkOptions;

        private readonly List<IOutputterAsync> _outputtersAsync = new List<IOutputterAsync>();


        public COutputManagerAsync(String defaultFilename)
        {
            _defaultFilename = defaultFilename.ThrowIfNullOrEmpty(nameof(defaultFilename));

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

        public Boolean Remove(IOutputterAsync item)
        {
            item.ThrowIfNull(nameof(item));
            return _outputtersAsync.Remove(item);
        }

        #endregion

        public async Task<Boolean> SaveResults(
            Dictionary<Type, BufferBlock<CRatingDataContainer>> resultsQueues, String storageName)
        {
            if (String.IsNullOrEmpty(storageName))
            {
                storageName = _defaultFilename;
            }

            var consumers = new List<ActionBlock<CRatingDataContainer>>();
            var results = new List<List<CRatingDataContainer>>();
            foreach (KeyValuePair<Type, BufferBlock<CRatingDataContainer>> keyValue in resultsQueues)
            {
                var rating = new List<CRatingDataContainer>();
                results.Add(rating);

                var consumer = new ActionBlock<CRatingDataContainer>(x => rating.Add(x),
                                                                     _consumerOptions);
                consumers.Add(consumer);
                keyValue.Value.LinkTo(consumer, _linkOptions);
            }

            await Task.WhenAll(consumers.Select(consumer => consumer.Completion));

            results.AsParallel().ForAll(
                rating => rating.Sort((x, y) => y.RatingValue.CompareTo(x.RatingValue))
            );

            List<Task<Boolean>> resultTasks = _outputtersAsync.Select(
                outputterAsync => outputterAsync.SaveResults(results, storageName)
            ).ToList();

            Boolean[] statuses = await Task.WhenAll(resultTasks);
            if (!statuses.IsNullOrEmpty() && statuses.All(r => r))
            {
                s_logger.Info($"Successfully saved all results to \"{storageName}\".");
                return true;
            }

            s_logger.Info($"Couldn't save some results to \"{storageName}\".");
            return false;
        }
    }
}
