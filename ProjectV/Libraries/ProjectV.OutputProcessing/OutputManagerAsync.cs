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
    /// <summary>
    /// Class which controlling results saving.
    /// </summary>
    public sealed class OutputManagerAsync : IManager<IOutputterAsync>
    {
        /// <summary>
        /// Logger instance for current class.
        /// </summary>
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor<OutputManagerAsync>();

        /// <summary>
        /// Default storage name if user will not specify it.
        /// </summary>
        private readonly string _defaultStorageName;

        /// <summary>
        /// Collection of concrete outputter classes which can save results to specified source.
        /// </summary>
        private readonly List<IOutputterAsync> _outputtersAsync = new List<IOutputterAsync>();


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
        public OutputManagerAsync(
            string defaultStorageName)
        {
            _defaultStorageName =
                defaultStorageName.ThrowIfNullOrWhiteSpace(nameof(defaultStorageName));
        }

        #region IManager<IOutputterAsync> Implementation

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">
        /// <paramref name="item" /> is <c>null</c>.
        /// </exception>
        public void Add(IOutputterAsync item)
        {
            item.ThrowIfNull(nameof(item));
            if (!_outputtersAsync.Contains(item))
            {
                _outputtersAsync.Add(item);
            }
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">
        /// <paramref name="item" /> is <c>null</c>.
        /// </exception>
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

            // A trick to complete pipeline with some pre-actions for outputters.
            // Now we only logging results because we should process all appraised results
            // before saving (e.g. sort them). So, may be in the future we will have some additional
            // logic instead of plain logging here.
            Action<RatingDataContainer> outputtersFunc = result =>
            {
                _logger.Info($"Got result for Thing:");
                _logger.Info($"    ThingId: '{result.DataHandler.ThingId.ToString()}'.");
                _logger.Info($"    Title: '{result.DataHandler.Title.ToString()}'.");
            };

            var outputtersFlow = new OutputtersFlow(new[] { outputtersFunc });

            _logger.Info("Constructed outputters pipeline.");
            return outputtersFlow;
        }

        /// <summary>
        /// Executes saving procedure and get it status as boolean variable. Results will be saved
        /// when the whole pipeline will be finished.
        /// </summary>
        /// <param name="results">Collections of appraised results to save.</param>
        /// <param name="storageName">Storage name of output source.</param>
        /// <returns><c>true</c> if the save was successful, <c>false</c> otherwise.</returns>
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
