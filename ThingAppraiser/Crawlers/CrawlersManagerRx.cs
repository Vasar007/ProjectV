using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using ThingAppraiser.Data;
using ThingAppraiser.Logging;

namespace ThingAppraiser.Crawlers
{
    public sealed class CCrawlersManagerRx : IManager<CCrawlerRx>
    {
        private static readonly CLoggerAbstraction s_logger =
            CLoggerAbstraction.CreateLoggerInstanceFor<CCrawlersManagerRx>();

        private readonly List<CCrawlerRx> _crawlersRx = new List<CCrawlerRx>();

        private readonly Boolean _outputResults;


        public CCrawlersManagerRx(Boolean outputResults)
        {
            _outputResults = outputResults;
        }

        #region IManager<CCrawlerRx> Implementation

        public void Add(CCrawlerRx item)
        {
            item.ThrowIfNull(nameof(item));
            if (!_crawlersRx.Contains(item))
            {
                _crawlersRx.Add(item);
            }
        }

        public Boolean Remove(CCrawlerRx item)
        {
            item.ThrowIfNull(nameof(item));
            return _crawlersRx.Remove(item);
        }

        #endregion

        public IObservable<CBasicInfo> CollectAllResponses(IObservable<String> entitiesQueue)
        {
            IObservable<CBasicInfo> responsesQueue = _crawlersRx.Select(crawlerRx =>
                entitiesQueue.ObserveOn(ThreadPoolScheduler.Instance).Select(
                    entity => Observable.FromAsync(
                        async () => await crawlerRx.FindResponse(entity, _outputResults)
                    )
                ).Concat()
            ).Merge();

            s_logger.Info("Crawlers were configured.");
            return responsesQueue;
        }
    }
}
