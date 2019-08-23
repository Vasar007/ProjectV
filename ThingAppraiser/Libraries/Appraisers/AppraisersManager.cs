using System;
using System.Collections.Generic;
using ThingAppraiser.Logging;
using ThingAppraiser.Communication;
using ThingAppraiser.Models.Data;
using ThingAppraiser.Models.Internal;

namespace ThingAppraiser.Appraisers
{
    /// <summary>
    /// Class which connects collected data with appraisers and executes the last one to process
    /// this data.
    /// </summary>
    public sealed class AppraisersManager : IManager<Appraiser>
    {
        /// <summary>
        /// Logger instance for current class.
        /// </summary>
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor<AppraisersManager>();

        /// <summary>
        /// Represents relations one-to-many for every type of the potential data.
        /// </summary>
        private readonly Dictionary<Type, List<Appraiser>> _appraisers =
            new Dictionary<Type, List<Appraiser>>();

        /// <summary>
        /// Sets this flag to <c>true</c> if you need to monitor appraisers results.
        /// </summary>
        private readonly bool _outputResults;

        /// <summary>
        /// Represents additional data structure to work with ratings.
        /// </summary>
        private readonly RatingsStorage _ratingsStorage = new RatingsStorage();


        /// <summary>
        /// Initializes manager for appraisers.
        /// </summary>
        /// <param name="outputResults">Flag to define need to output appraisers results.</param>
        public AppraisersManager(bool outputResults)
        {
            _outputResults = outputResults;
        }

        #region IManager<Appraiser> Implementation

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">
        /// <paramref name="item" /> is <c>null</c>.
        /// </exception>
        public void Add(Appraiser item)
        {
            item.ThrowIfNull(nameof(item));

            if (_appraisers.TryGetValue(item.TypeId, out List<Appraiser> list))
            {
                if (!list.Contains(item))
                {
                    item.RatingId = _ratingsStorage.Register(item.TypeId, item.RatingName);
                    list.Add(item);
                }
            }
            else
            {
                item.RatingId = _ratingsStorage.Register(item.TypeId, item.RatingName);
                _appraisers.Add(item.TypeId, new List<Appraiser> { item });
            }
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">
        /// <paramref name="item" /> is <c>null</c>.
        /// </exception>
        public bool Remove(Appraiser item)
        {
            item.ThrowIfNull(nameof(item));

            if (!_ratingsStorage.Deregister(item.RatingId))
            {
                _logger.Warn("Removed appraiser had unregistered rating ID.");
            }
            return _appraisers.Remove(item.TypeId);
        }

        #endregion

        /// <summary>
        /// Finds suitable appraisers for every collection and execute ratings calculations.
        /// </summary>
        /// <param name="data">Collections of crawlers results.</param>
        /// <returns>Appraised collections produced from a set of data.</returns>
        public ProcessedDataContainer GetAllRatings(List<RawDataContainer> data)
        {
            var results = new List<ResultList>();
            foreach (RawDataContainer datum in data)
            {
                IReadOnlyList<BasicInfo> internalData = datum.GetData();
                // Skip empty collections of data.
                if (internalData.IsNullOrEmpty()) continue;

                // Suggest that all types in collection are identical.
                if (!_appraisers.TryGetValue(internalData[0].GetType(),
                                             out List<Appraiser> values))
                {
                    string message = $"Type {internalData[0].GetType()} wasn't used to appraise!";
                    _logger.Info(message);
                    GlobalMessageHandler.OutputMessage(message);
                    continue;
                }

                foreach (Appraiser appraiser in values)
                {
                    results.Add(appraiser.GetRatings(datum, _outputResults));
                }
            }
            return new ProcessedDataContainer(results, _ratingsStorage);
        }
    }
}
