using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using ThingAppraiser.Data;
using ThingAppraiser.Logging;

namespace ThingAppraiser.Crawlers
{
    public sealed class CrawlersManagerRx : IManager<CrawlerRx>
    {
        private static readonly LoggerAbstraction _logger =
            LoggerAbstraction.CreateLoggerInstanceFor<CrawlersManagerRx>();

        private readonly List<CrawlerRx> _crawlersRx = new List<CrawlerRx>();

        private readonly bool _outputResults;


        public CrawlersManagerRx(bool outputResults)
        {
            _outputResults = outputResults;
        }

        #region IManager<CrawlerRx> Implementation

        public void Add(CrawlerRx item)
        {
            item.ThrowIfNull(nameof(item));
            if (!_crawlersRx.Contains(item))
            {
                _crawlersRx.Add(item);
            }
        }

        public bool Remove(CrawlerRx item)
        {
            item.ThrowIfNull(nameof(item));
            return _crawlersRx.Remove(item);
        }

        #endregion

        public IDictionary<Type, IObservable<BasicInfo>> CollectAllResponses(
            IObservable<string> entitiesQueue)
        {
            var responsesQueues = new Dictionary<Type, IObservable<BasicInfo>>();

            foreach (CrawlerRx crawlerRx in _crawlersRx)
            {
                var responseQueue = entitiesQueue.ObserveOn(ThreadPoolScheduler.Instance).Select(
                    entity => Observable.FromAsync(
                        async () => await crawlerRx.FindResponse(entity, _outputResults)
                    )
                    .Where(x => !(x is null))
                ).Concat();

                responsesQueues.Add(crawlerRx.TypeId, responseQueue);
            }

            _logger.Info("Crawlers were configured.");
            return responsesQueues;
        }
    }
}
