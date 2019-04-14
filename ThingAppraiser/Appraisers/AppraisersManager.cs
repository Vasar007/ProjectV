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


        /// <summary>
        /// Initializes manager for appraisers.
        /// </summary>
        /// <param name="outputResults">Flag to define need to output appraisers results.</param>
        public CAppraisersManager(Boolean outputResults)
        {
            _outputResults = outputResults;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public CAppraisersManager()
            : this(false)
        {
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
                    list.Add(item);
                }
            }
            else
            {
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
            return _appraisers.Remove(item.TypeID);
        }

        #endregion

        /// <summary>
        /// Finds suitable appraisers for every collection and execute ratings calculations.
        /// </summary>
        /// <param name="data">Collections of crawlers results.</param>
        /// <returns>Appraised collections produced from a set of data.</returns>
        public List<CRating> GetAllRatings(List<List<CBasicInfo>> data)
        {
            var results = new List<CRating>();
            foreach (List<CBasicInfo> datum in data)
            {
                // Skip empty collections of data.
                if (datum.Count == 0) continue;

                // Suggest that all types in collection are identical.
                if (!_appraisers.TryGetValue(datum[0].GetType(), out List<CAppraiser> values))
                {
                    s_logger.Info($"Type {datum[0].GetType()} wasn't used to appraise!");
                    SGlobalMessageHandler.OutputMessage(
                        $"Type {datum[0].GetType()} wasn't used to appraise!"
                    );
                    continue;
                }
                foreach (CAppraiser appraiser in values)
                {
                    results.Add(appraiser.GetRatings(datum, _outputResults));
                }
            }
            return results;
        }
    }
}
