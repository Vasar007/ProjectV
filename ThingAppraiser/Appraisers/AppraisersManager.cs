using System;
using System.Collections;
using System.Collections.Generic;

namespace ThingAppraiser.Appraisers
{
    /// <summary>
    /// Class which connects collected data with appraisers and executes the last one to process
    /// this data.
    /// </summary>
    /// <remarks>
    /// Class implements <see cref="IEnumerable"> to simplify call to the constructor with 
    /// collection initializer.
    /// </remarks>
    public class AppraisersManager : IEnumerable
    {
        #region Const & Static Fields

        /// <summary>
        /// Logger instance for current class.
        /// </summary>
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        #region Class Fields

        /// <summary>
        /// Represents relations one-to-many for every type of the potential data.
        /// </summary>
        private Dictionary<Type, List<Appraiser>> _appraisers =
            new Dictionary<Type, List<Appraiser>>();

        #endregion

        #region Public Class Methods

        /// <summary>
        /// Add appraiser to collection.
        /// </summary>
        /// <param name="appraiser">Concrete appraiser implementation.</param>
        public void Add(Appraiser appraiser)
        {
            appraiser.ThrowIfNull(nameof(appraiser));

            if (_appraisers.TryGetValue(appraiser.TypeID, out var list))
            {
                if (!list.Contains(appraiser))
                {
                    list.Add(appraiser);
                }
            }
            else
            {
                _appraisers.Add(appraiser.TypeID, new List<Appraiser> { appraiser });
            }
        }

        /// <summary>
        /// Find suitable appraisers for every collection and execute ratings calculations.
        /// </summary>
        /// <param name="data">Collections of crawlers results.</param>
        /// <returns>Appraised collections produced from a set of data.</returns>
        public List<List<Data.ResultType>> GetAllRatings(List<List<Data.DataHandler>> data)
        {
            var results = new List<List<Data.ResultType>>();
            foreach (var datum in data)
            {
                // Skip empty collections of data.
                if (datum.Count == 0) continue;

                // Suggest that all types in collection are identical.
                if (!_appraisers.TryGetValue(datum[0].GetType(), out var values))
                {
                    _logger.Info($"Type {datum[0].GetType()} wasn't used to appraise!");
                    Core.Shell.OutputMessage($"Type {datum[0].GetType()} wasn't used to appraise!");
                    continue;
                }
                foreach (var appraiser in values)
                {
                    results.Add(appraiser.GetRatings(datum));
                }
            }
            return results;
        }

        #endregion

        #region Impements IEnumerable

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A collection enumerator.</returns>
        public IEnumerator GetEnumerator()
        {
            return _appraisers.GetEnumerator();
        }

        #endregion
    }
}
