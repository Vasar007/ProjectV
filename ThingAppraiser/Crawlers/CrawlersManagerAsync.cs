using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using ThingAppraiser.Data;
using ThingAppraiser.Logging;

namespace ThingAppraiser.Crawlers
{
    public sealed class CCrawlersManagerAsync : IManager<CCrawlerAsync>
    {
        private static readonly CLoggerAbstraction s_logger =
            CLoggerAbstraction.CreateLoggerInstanceFor<CCrawlersManagerAsync>();

        private readonly List<CCrawlerAsync> _crawlersAsync = new List<CCrawlerAsync>();

        private readonly Boolean _outputResults;


        public CCrawlersManagerAsync(Boolean outputResults)
        {
            _outputResults = outputResults;
        }

        #region IManager<CCrawlerAsync> Implementation

        public void Add(CCrawlerAsync item)
        {
            item.ThrowIfNull(nameof(item));
            if (!_crawlersAsync.Contains(item))
            {
                _crawlersAsync.Add(item);
            }
        }

        public Boolean Remove(CCrawlerAsync item)
        {
            item.ThrowIfNull(nameof(item));
            return _crawlersAsync.Remove(item);
        }

        #endregion

        public async Task<Boolean> CollectAllResponses(BufferBlock<String> entitiesQueue,
            Dictionary<Type, BufferBlock<CBasicInfo>> responsesQueues, DataflowBlockOptions options)
        {
            var producers = new List<Task<Boolean>>();
            foreach (CCrawlerAsync crawlerAsync in _crawlersAsync)
            {
                var responseQueue = new BufferBlock<CBasicInfo>(options);
                responsesQueues.Add(crawlerAsync.TypeID, responseQueue);
                producers.Add(crawlerAsync.GetResponse(entitiesQueue, responseQueue,
                                                       _outputResults));
            }

            Boolean[] statuses =  await Task.WhenAll(producers);
            foreach (BufferBlock<CBasicInfo> responsesQueue in responsesQueues.Values)
            {
                responsesQueue.Complete();
            }

            if (!statuses.IsNullOrEmpty() && statuses.All(r => r))
            {
                s_logger.Info("Crawlers have finished work.");
                return true;
            }

            s_logger.Info("Crawlers have not received any data.");
            return false;
        }
    }
}
