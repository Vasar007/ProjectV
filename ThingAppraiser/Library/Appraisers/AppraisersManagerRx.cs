using System;
using System.Linq;
using System.Collections.Generic;
using System.Reactive.Linq;
using ThingAppraiser.Data;
using ThingAppraiser.Logging;
using ThingAppraiser.Communication;

namespace ThingAppraiser.Appraisers
{
    public sealed class AppraisersManagerRx : IManager<AppraiserRx>
    {
        private static readonly LoggerAbstraction _logger =
            LoggerAbstraction.CreateLoggerInstanceFor<AppraisersManagerRx>();

        private readonly Dictionary<Type, List<AppraiserRx>> _appraisersRx =
            new Dictionary<Type, List<AppraiserRx>>();

        private readonly bool _outputResults;


        public AppraisersManagerRx(bool outputResults)
        {
            _outputResults = outputResults;
        }

        #region IManager<AppraiserRx> Implementation

        public void Add(AppraiserRx item)
        {
            item.ThrowIfNull(nameof(item));

            if (_appraisersRx.TryGetValue(item.TypeId, out List<AppraiserRx> list))
            {
                if (!list.Contains(item))
                {
                    list.Add(item);
                }
            }
            else
            {
                _appraisersRx.Add(item.TypeId, new List<AppraiserRx> { item });
            }
        }

        public bool Remove(AppraiserRx item)
        {
            item.ThrowIfNull(nameof(item));
            return _appraisersRx.Remove(item.TypeId);
        }

        #endregion

        public IDictionary<Type, IObservable<RatingDataContainer>> GetAllRatings(
            IDictionary<Type, IObservable<BasicInfo>> entitiesInfoQueues)
        {
            var ratingsQueues = new Dictionary<Type, IObservable<RatingDataContainer>>();

            foreach (KeyValuePair<Type, IObservable<BasicInfo>> keyValue in entitiesInfoQueues)
            {
                if (!_appraisersRx.TryGetValue(keyValue.Key, out List<AppraiserRx> values))
                {
                    string message = $"Type {keyValue.Key} wasn't used to appraise!";
                    _logger.Info(message);
                    GlobalMessageHandler.OutputMessage(message);
                    continue;
                }

                var entitiesRatingQueue = values.Select(appraisersRx =>
                    keyValue.Value.Select(
                        basicInfo => appraisersRx.GetRatings(basicInfo, _outputResults)
                    )
                ).Concat();

                ratingsQueues.Add(keyValue.Key, entitiesRatingQueue);
            }

            _logger.Info("Appraisers were configured.");
            return ratingsQueues;
        }
    }
}
