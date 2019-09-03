using System;
using System.Collections.Generic;
using System.Linq;
using ThingAppraiser.Communication;
using ThingAppraiser.Logging;
using ThingAppraiser.Models.Data;
using ThingAppraiser.Models.Internal;

namespace ThingAppraiser.Appraisers
{
    /// <summary>
    /// Class which connects collected data with appraisers and executes the last one to process
    /// this data.
    /// </summary>
    public sealed class AppraisersManager : IManager<IAppraiser>
    {
        /// <summary>
        /// Logger instance for current class.
        /// </summary>
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor<AppraisersManager>();

        /// <summary>
        /// Represents relations one-to-many for every type of the potential data.
        /// </summary>
        private readonly Dictionary<Type, IList<IAppraiser>> _appraisers =
            new Dictionary<Type, IList<IAppraiser>>();

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
        public void Add(IAppraiser item)
        {
            item.ThrowIfNull(nameof(item));

            if (_appraisers.TryGetValue(item.TypeId, out IList<IAppraiser> list))
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
                _appraisers.Add(item.TypeId, new List<IAppraiser> { item });
            }
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">
        /// <paramref name="item" /> is <c>null</c>.
        /// </exception>
        public bool Remove(IAppraiser item)
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
        /// <exception cref="ArgumentNullException">
        /// <paramref name="data" /> is <c>null</c>.
        /// </exception>
        public ProcessedDataContainer GetAllRatings(IReadOnlyList<RawDataContainer> data)
        {
            data.ThrowIfNull(nameof(data));

            var results = new List<IReadOnlyList<ResultInfo>>();
            foreach (RawDataContainer datum in data)
            {
                IReadOnlyList<BasicInfo> internalData = datum.RawData;
                // Skip empty collections of data.
                if (!internalData.Any()) continue;

                // Suggest that all types in collection are identical.
                Type itemsType = internalData.First().GetType();

                if (!_appraisers.TryGetValue(itemsType, out IList<IAppraiser> values))
                {
                    string message = $"Type {itemsType} was not used to appraise!";
                    _logger.Info(message);
                    GlobalMessageHandler.OutputMessage(message);
                    continue;
                }

                foreach (IAppraiser appraiser in values)
                {
                    results.Add(appraiser.GetRatings(datum, _outputResults));
                }
            }
            return new ProcessedDataContainer(results, _ratingsStorage);
        }
    }
}
