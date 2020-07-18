using System;
using System.Collections.Generic;
using Acolyte.Assertions;
using ProjectV.DataPipeline;
using ProjectV.Logging;
using ProjectV.Models.Data;
using ProjectV.Models.Internal;

namespace ProjectV.Appraisers
{
    public sealed class AppraisersManagerAsync : IManager<IAppraiserAsync>
    {
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor<AppraisersManagerAsync>();

        private readonly Dictionary<Type, IList<IAppraiserAsync>> _appraisersAsync =
            new Dictionary<Type, IList<IAppraiserAsync>>();

        private readonly bool _outputResults;


        public AppraisersManagerAsync(bool outputResults)
        {
            _outputResults = outputResults;
        }

        #region IManager<AppraiserAsync> Implementation

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
