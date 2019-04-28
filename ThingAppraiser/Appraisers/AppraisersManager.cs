using System;
using System.Collections.Generic;
using ThingAppraiser.Logging;
using ThingAppraiser.Data;
using ThingAppraiser.Communication;

namespace ThingAppraiser.Appraisers
{
    /// <summary>
    /// Class which connects collected data with appraisers and executes the last one to process
    /// this data.
    /// </summary>
    public sealed class CAppraisersManager : IManager<CAppraiser>
    {
        /// <summary>
        /// Logger instance for current class.
        /// </summary>
        private static readonly CLoggerAbstraction s_logger =
            CLoggerAbstraction.CreateLoggerInstanceFor<CAppraisersManager>();

        /// <summary>
        /// Represents relations one-to-many for every type of the potential data.
        /// </summary>
        private readonly Dictionary<Type, List<CAppraiser>> _appraisers =
            new Dictionary<Type, List<CAppraiser>>();

        /// <summary>
        /// Sets this flag to <c>true</c> if you need to monitor appraisers results.
        /// </summary>
        private readonly Boolean _outputResults;

        private readonly CRatingsStorage _ratingsStorage = new CRatingsStorage();


        /// <summary>
        /// Initializes manager for appraisers.
        /// </summary>
        /// <param name="outputResults">Flag to define need to output appraisers results.</param>
        public CAppraisersManager(Boolean outputResults)
        {
            _outputResults = outputResults;
        }

        #region IManager<CAppraiser> Implementation

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">
        /// <paramref name="item">item</paramref> is <c>null</c>.
        /// </exception>
        public void Add(CAppraiser item)
        {
            item.ThrowIfNull(nameof(item));

            if (_appraisers.TryGetValue(item.TypeID, out List<CAppraiser> list))
            {
                if (!list.Contains(item))
                {
                    item.RatingID = _ratingsStorage.Register(item.TypeID, item.RatingName);
                    list.Add(item);
                }
            }
            else
            {
                item.RatingID = _ratingsStorage.Register(item.TypeID, item.RatingName);
                _appraisers.Add(item.TypeID, new List<CAppraiser> { item });
            }
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">
        /// <paramref name="item">item</paramref> is <c>null</c>.
        /// </exception>
        public Boolean Remove(CAppraiser item)
        {
            item.ThrowIfNull(nameof(item));

            if (!_ratingsStorage.Deregister(item.RatingID))
            {
                s_logger.Warn("Removed appraiser had unregistered rating ID.");
            }
            return _appraisers.Remove(item.TypeID);
        }

        #endregion

        /// <summary>
        /// Finds suitable appraisers for every collection and execute ratings calculations.
        /// </summary>
        /// <param name="data">Collections of crawlers results.</param>
        /// <returns>Appraised collections produced from a set of data.</returns>
        public CProcessedDataContainer GetAllRatings(List<CRawDataContainer> data)
        {
            var results = new List<CResultList>();
            foreach (CRawDataContainer datum in data)
            {
                IReadOnlyList<CBasicInfo> internalData = datum.GetData();
                // Skip empty collections of data.
                if (internalData.IsNullOrEmpty()) continue;

                // Suggest that all types in collection are identical.
                if (!_appraisers.TryGetValue(internalData[0].GetType(),
                                             out List<CAppraiser> values))
                {
                    s_logger.Info($"Type {internalData[0].GetType()} wasn't used to appraise!");
                    SGlobalMessageHandler.OutputMessage(
                        $"Type {internalData[0].GetType()} wasn't used to appraise!"
                    );
                    continue;
                }

                foreach (CAppraiser appraiser in values)
                {
                    results.Add(appraiser.GetRatings(datum, _outputResults));
                }
            }
            return new CProcessedDataContainer(results, _ratingsStorage);
        }
    }
}
