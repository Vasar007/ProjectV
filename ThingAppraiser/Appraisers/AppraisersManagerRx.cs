using System;
using System.Linq;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using ThingAppraiser.Data;
using ThingAppraiser.Logging;

namespace ThingAppraiser.Appraisers
{
    public sealed class CAppraisersManagerRx : IManager<CAppraiserRx>
    {
        private static readonly CLoggerAbstraction s_logger =
            CLoggerAbstraction.CreateLoggerInstanceFor<CAppraisersManagerRx>();

        private readonly Dictionary<Type, List<CAppraiserRx>> _appraisersRx =
            new Dictionary<Type, List<CAppraiserRx>>();

        private readonly Boolean _outputResults;


        public CAppraisersManagerRx(Boolean outputResults)
        {
            _outputResults = outputResults;
        }

        #region IManager<CAppraiserRx> Implementation

        public void Add(CAppraiserRx item)
        {
            item.ThrowIfNull(nameof(item));

            if (_appraisersRx.TryGetValue(item.TypeID, out List<CAppraiserRx> list))
            {
                if (!list.Contains(item))
                {
                    list.Add(item);
                }
            }
            else
            {
                _appraisersRx.Add(item.TypeID, new List<CAppraiserRx> { item });
            }
        }

        public Boolean Remove(CAppraiserRx item)
        {
            item.ThrowIfNull(nameof(item));
            return _appraisersRx.Remove(item.TypeID);
        }

        #endregion

        public IObservable<CRatingDataContainer> GetAllRatings(IObservable<CBasicInfo> entitiesInfoQueue)
        {
            IObservable<CRatingDataContainer> entitiesRatingQueues =
                entitiesInfoQueue.ObserveOn(ThreadPoolScheduler.Instance)
                    .Where(basicInfo => !(basicInfo is null) &&
                                        _appraisersRx.Any(kv => kv.Key == basicInfo.GetType()))
                    .Select(basicInfo =>
                    {
                        List<CAppraiserRx> values = _appraisersRx[basicInfo.GetType()];
                        return values.Select(appraisersRx => appraisersRx.GetRatings(
                            basicInfo, _outputResults)
                        ).ToObservable();
                    }
                ).Merge();

            s_logger.Info("Appraisers were configured.");
            return entitiesRatingQueues;
        }
    }
}
