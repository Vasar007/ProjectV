using System;
using System.Collections.Generic;
using Acolyte.Assertions;
using ProjectV.DataPipeline;
using ProjectV.Logging;
using ProjectV.Models.Data;
using ProjectV.Models.Internal;

namespace ProjectV.Appraisers
{
    /// <summary>
    /// Class which connects collected data with appraisers and executes the last ones to process
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
        /// Initializes manager for appraisers.
        /// </summary>
        /// <param name="outputResults">Flag to define need to output appraisers results.</param>
        public AppraisersManager(
            bool outputResults)
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
                    list.Add(item);
                }
            }
            else
            {
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
            return _appraisers.Remove(item.TypeId);
        }

        #endregion

        public AppraisersFlow CreateFlow()
        {
            var appraisersFunc = new List<Funcotype>(_appraisers.Count);
            foreach ((Type type, IList<IAppraiser> appraisers) in _appraisers)
            {
                foreach (IAppraiser appraiserAsync in appraisers)
                {
                    var funcotype = new Funcotype(
                        entityInfo => TryGetRatings(appraiserAsync, entityInfo),
                        type
                    );
                    appraisersFunc.Add(funcotype);
                }
            }

            var appraisersFlow = new AppraisersFlow(appraisersFunc);

            _logger.Info("Constructed appraisers pipeline.");
            return appraisersFlow;
        }

        /// <summary>
        /// Execute rating calculation for suitable appraisers.
        /// </summary>
        /// <param name="appraisers">Appraiser to execute calculation.</param>
        /// <param name="entityInfo">Entity info from crawler to apprais.</param>
        /// <returns>Appraised data produced from an entity info.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="data" /> is <c>null</c>.
        /// </exception>
        private RatingDataContainer TryGetRatings(IAppraiser appraisers,
           BasicInfo entityInfo)
        {
            try
            {
                return appraisers.GetRatings(entityInfo, _outputResults);
            }
            catch (Exception ex)
            {
                string message = $"Appraiser {appraisers.Tag} could not process " +
                                 $"entity info \"{entityInfo.Title}\".";
                _logger.Error(ex, message);
                throw;
            }
        }
    }
}
