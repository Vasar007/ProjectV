using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using ThingAppraiser.Data;
using ThingAppraiser.Logging;

namespace ThingAppraiser.Crawlers
{
    public sealed class CrawlersManagerAsync : IManager<CrawlerAsync>
    {
        private static readonly LoggerAbstraction _logger =
            LoggerAbstraction.CreateLoggerInstanceFor<CrawlersManagerAsync>();

        private readonly List<CrawlerAsync> _crawlersAsync = new List<CrawlerAsync>();

        private readonly bool _outputResults;


        public CrawlersManagerAsync(bool outputResults)
        {
            _outputResults = outputResults;
        }

        #region IManager<CrawlerAsync> Implementation

        public void Add(CrawlerAsync item)
        {
            item.ThrowIfNull(nameof(item));
            if (!_crawlersAsync.Contains(item))
            {
                _crawlersAsync.Add(item);
            }
        }

        public bool Remove(CrawlerAsync item)
        {
            item.ThrowIfNull(nameof(item));
            return _crawlersAsync.Remove(item);
        }

        #endregion

        public async Task<bool> CollectAllResponses(BufferBlock<string> entitiesQueue,
            IDictionary<Type, BufferBlock<BasicInfo>> responsesQueues, DataflowBlockOptions options)
        {
            var producers = new List<Task<bool>>();
            foreach (CrawlerAsync crawlerAsync in _crawlersAsync)
            {
                var responseQueue = new BufferBlock<BasicInfo>(options);
                responsesQueues.Add(crawlerAsync.TypeId, responseQueue);
                producers.Add(crawlerAsync.GetResponse(entitiesQueue, responseQueue,
                                                       _outputResults));
            }

            bool[] statuses =  await Task.WhenAll(producers);
            foreach (BufferBlock<BasicInfo> responsesQueue in responsesQueues.Values)
            {
                responsesQueue.Complete();
            }

            if (!statuses.IsNullOrEmpty() && statuses.All(r => r))
            {
                _logger.Info("Crawlers have finished work.");
                return true;
            }

            _logger.Info("Crawlers have not received any data.");
            return false;
        }
    }
}
