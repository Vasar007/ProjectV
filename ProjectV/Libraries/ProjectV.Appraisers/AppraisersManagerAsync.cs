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
    public sealed class AppraisersManagerAsync : IManager<IAppraiserAsync>
    {
        /// <summary>
        /// Logger instance for current class.
        /// </summary>
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor<AppraisersManagerAsync>();

        /// <summary>
        /// Represents relations one-to-many for every type of the potential data.
        /// </summary>
        private readonly Dictionary<Type, IList<IAppraiserAsync>> _appraisersAsync =
            new Dictionary<Type, IList<IAppraiserAsync>>();

        /// <summary>
        /// Sets this flag to <c>true</c> if you need to monitor appraisers results.
        /// </summary>
        private readonly bool _outputResults;


        /// <summary>
        /// Initializes manager for appraisers.
        /// </summary>
        /// <param name="outputResults">Flag to define need to output appraisers results.</param>
        public AppraisersManagerAsync(
            bool outputResults)
        {
            _outputResults = outputResults;
        }

        #region IManager<AppraiserAsync> Implementation

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">
        /// <paramref name="item" /> is <c>null</c>.
        /// </exception>
        public void Add(IAppraiserAsync item)
        {
            item.ThrowIfNull(nameof(item));

            if (_appraisersAsync.TryGetValue(item.TypeId, out IList<IAppraiserAsync> list))
            {
                if (!list.Contains(item))
                {
                    list.Add(item);
                }
            }
            else
            {
                _appraisersAsync.Add(item.TypeId, new List<IAppraiserAsync> { item });
            }
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">
        /// <paramref name="item" /> is <c>null</c>.
        /// </exception>
        public bool Remove(IAppraiserAsync item)
        {
            item.ThrowIfNull(nameof(item));
            return _appraisersAsync.Remove(item.TypeId);
        }

        #endregion

        public AppraisersFlow CreateFlow()
        {
            var appraisersFunc = new List<Funcotype>(_appraisersAsync.Count);
            foreach ((Type type, IList<IAppraiserAsync> appraisersAsync) in _appraisersAsync)
            {
                foreach (IAppraiserAsync appraiserAsync in appraisersAsync)
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
        /// <param name="appraisersAsync">Appraiser to execute calculation.</param>
        /// <param name="entityInfo">Entity info from crawler to apprais.</param>
        /// <returns>Appraised data produced from an entity info.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="data" /> is <c>null</c>.
        /// </exception>
        private RatingDataContainer TryGetRatings(IAppraiserAsync appraisersAsync,
           BasicInfo entityInfo)
        {
            try
            {
                return appraisersAsync.GetRatings(entityInfo, _outputResults);
            }
            catch (Exception ex)
            {
                string message = $"Appraiser {appraisersAsync.Tag} could not process " +
                                 $"entity info \"{entityInfo.Title}\".";
                _logger.Error(ex, message);
                throw;
            }
        }
    }
}
