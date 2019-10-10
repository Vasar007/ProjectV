using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using ThingAppraiser.DataPipeline;
using ThingAppraiser.Extensions;
using ThingAppraiser.Logging;
using ThingAppraiser.Models.Internal;

namespace ThingAppraiser.IO.Output
{
    public sealed class OutputManagerAsync : IManager<IOutputterAsync>
    {
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor<OutputManagerAsync>();

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

        public OutputtersFlow CreateFlow(string storageName)
        {
            if (string.IsNullOrWhiteSpace(storageName))
            {
                storageName = _defaultStorageName;

                _logger.Info("Storage name is empty, using the default value.");
            }

            var outputtersFunc = _outputtersAsync.Select(outputterAsync =>
            {
                return new Action<RatingDataContainer>(result => Console.WriteLine("Got result."));
            });

            var outputtersFlow = new OutputtersFlow(outputtersFunc.Take(1));

            _logger.Info("Constructed outputters pipeline.");
            return outputtersFlow;
        }

        public async Task<bool> SaveResults(OutputtersFlow outputtersFlow, string storageName)
        {
            if (string.IsNullOrWhiteSpace(storageName))
            {
                storageName = _defaultStorageName;

                _logger.Info("Storage name is empty, using the default value.");
            }

            await outputtersFlow.CompletionTask;

            IReadOnlyList<RatingDataContainer> results = outputtersFlow.Results.ToReadOnlyList();

            IReadOnlyList<List<RatingDataContainer>> consumedResults = results
                .GroupBy(
                    rating => rating.RatingId,
                    (key, group) => group.ToList()
                )
                .ToReadOnlyList();

            consumedResults.AsParallel().ForAll(
                rating => rating.Sort((x, y) => y.RatingValue.CompareTo(x.RatingValue))
            );

            IReadOnlyList<Task<bool>> resultTasks = _outputtersAsync.Select(
                outputterAsync => outputterAsync.SaveResults(consumedResults, storageName)
            ).ToReadOnlyList();

            IReadOnlyList<bool> statuses = await Task.WhenAll(resultTasks);
            if (statuses.Count > 0 && statuses.All(r => r))
            {
                _logger.Info($"Successfully saved all results to \"{storageName}\".");
                return true;
            }

            _logger.Info($"Could not save some results to \"{storageName}\".");
            return false;
        }
    }
}
