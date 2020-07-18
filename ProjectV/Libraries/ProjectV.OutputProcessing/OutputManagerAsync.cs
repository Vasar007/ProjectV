using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Acolyte.Assertions;
using Acolyte.Collections;
using ProjectV.Communication;
using ProjectV.DataPipeline;
using ProjectV.Logging;
using ProjectV.Models.Internal;

namespace ProjectV.IO.Output
{
    public sealed class OutputManagerAsync : IManager<IOutputterAsync>
    {
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor<OutputManagerAsync>();

        private readonly string _defaultStorageName;

        private readonly List<IOutputterAsync> _outputtersAsync = new List<IOutputterAsync>();


        public OutputManagerAsync(string defaultStorageName)
        {
            _defaultStorageName =
                defaultStorageName.ThrowIfNullOrWhiteSpace(nameof(defaultStorageName));
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

                const string message = "Storage name is empty, using the default value.";
                _logger.Info(message);
                GlobalMessageHandler.OutputMessage(message);
            }

            // Make sure that the final pipeline task is completed.
            await outputtersFlow.CompletionTask;

            IReadOnlyList<RatingDataContainer> results = outputtersFlow.Results.ToReadOnlyList();

            IReadOnlyList<List<RatingDataContainer>> resultsToSave = results
                .GroupBy(rating => rating.RatingId, (key, group) => group.ToList())
                .ToReadOnlyList();

            resultsToSave
                .AsParallel()
                .ForAll(rating => rating.Sort((x, y) => y.RatingValue.CompareTo(x.RatingValue)));

            IReadOnlyList<Task<bool>> resultTasks = _outputtersAsync
                .Select(outputterAsync => TryGetRatings(outputterAsync, resultsToSave, storageName))
                .ToReadOnlyList();

            IReadOnlyList<bool> statuses = await Task.WhenAll(resultTasks);
            if (statuses.Count > 0 && statuses.All(statis => statis))
            {
                _logger.Info($"Successfully saved all results to \"{storageName}\".");
                return true;
            }

            _logger.Info($"Could not save some results to \"{storageName}\".");
            return false;
        }

        private static async Task<bool> TryGetRatings(IOutputterAsync outputterAsync,
           IReadOnlyList<IReadOnlyList<RatingDataContainer>> resultsToSave, string storageName)
        {
            try
            {
                return await outputterAsync.SaveResults(resultsToSave, storageName);
            }
            catch (Exception ex)
            {
                string message = $"Outputter {outputterAsync.Tag} could not save " +
                                 $"{resultsToSave.Count.ToString()} results to \"{storageName}\".";
                _logger.Error(ex, message);
                throw;
            }
        }
    }
}
