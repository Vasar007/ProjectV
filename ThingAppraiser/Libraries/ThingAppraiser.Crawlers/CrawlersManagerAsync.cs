using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using ThingAppraiser.Communication;
using ThingAppraiser.Extensions;
using ThingAppraiser.Logging;
using ThingAppraiser.Models.Data;

namespace ThingAppraiser.Crawlers
{
    public sealed class CrawlersManagerAsync : IManager<CrawlerAsync>, IDisposable
    {
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor<CrawlersManagerAsync>();

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

        public async Task<bool> CollectAllResponses(ISourceBlock<string> entitiesQueue,
            IDictionary<Type, BufferBlock<BasicInfo>> rawDataQueues, DataflowBlockOptions options)
        {
            var producers = new List<Task<bool>>(_crawlersAsync.Count);
            var consumers = new List<ITargetBlock<string>>(_crawlersAsync.Count);

            foreach (CrawlerAsync crawlerAsync in _crawlersAsync)
            {
                var consumer = new BufferBlock<string>(options);
                var rawDataQueue = new BufferBlock<BasicInfo>(options);

                rawDataQueues.Add(crawlerAsync.TypeId, rawDataQueue);
                producers.Add(crawlerAsync.GetResponse(consumer, rawDataQueue, _outputResults));
                consumers.Add(consumer);
            }

            Task<ResultOrException<bool>[]> statusesTask =
                TaskHelper.WhenAllResultsOrExceptions(producers);

            Task consumersTasks = Task.WhenAll(consumers.Select(consumer => consumer.Completion));
            Task splitQueueTask = SplitQueue(entitiesQueue, consumers);

            await Task.WhenAll(splitQueueTask, consumersTasks, statusesTask);

            (IReadOnlyList<bool> statuses, IReadOnlyList<Exception> taskExceptions) =
                statusesTask.Result.UnwrapResultsOrExceptions();

            // Need to release queues before results and exceptions processing.
            MarkAsCompleteQueues(rawDataQueues.Values);

            CheckExceptions(taskExceptions);

            if (!statuses.IsNullOrEmpty() && statuses.All(r => r))
            {
                _logger.Info("Crawlers have finished work.");
                return true;
            }

            _logger.Info("Crawlers have not received some data.");
            return false;
        }

        private async Task SplitQueue(ISourceBlock<string> entitiesQueue,
            IReadOnlyList<ITargetBlock<string>> consumers)
        {
            while (await entitiesQueue.OutputAvailableAsync())
            {
                string entity = await entitiesQueue.ReceiveAsync();

                if (_outputResults)
                {
                    GlobalMessageHandler.OutputMessage(
                        $"Got {entity} and transmitted to crawling."
                    );
                }

                await Task.WhenAll(
                    consumers.Select(async consumer => await consumer.SendAsync(entity))
                );
            }

            foreach (ITargetBlock<string> consumer in consumers)
            {
                consumer.Complete();
            }
        }

        private static void MarkAsCompleteQueues(IEnumerable<BufferBlock<BasicInfo>> queues)
        {
            foreach (BufferBlock<BasicInfo> rawDataQueue in queues)
            {
                rawDataQueue.Complete();
            }
        }

        private static void CheckExceptions(IReadOnlyList<Exception> taskExceptions)
        {
            if (!taskExceptions.Any()) return;

            if (taskExceptions.Count == 1)
            {
                throw new Exception($"One of the crawlers failed.", taskExceptions.Single());
            }

            throw new AggregateException(
                $"Some crawlers failed. Exceptions number: {taskExceptions.Count.ToString()}.",
                taskExceptions
            );
        }



        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
