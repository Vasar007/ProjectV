using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using ThingAppraiser.Communication;
using ThingAppraiser.Extensions;
using ThingAppraiser.Logging;
using ThingAppraiser.Models.Data;

namespace ThingAppraiser.Crawlers
{
    public sealed class CrawlersManagerAsync : IManager<ICrawlerAsync>, IDisposable
    {
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor<CrawlersManagerAsync>();

        private readonly List<ICrawlerAsync> _crawlersAsync = new List<ICrawlerAsync>();

        private readonly bool _outputResults;

        private readonly CancellationTokenSource _cancellationTokenSource =
            new CancellationTokenSource();

        private bool _disposed;


        public CrawlersManagerAsync(bool outputResults)
        {
            _outputResults = outputResults;
        }

        #region IManager<CrawlerAsync> Implementation

        public void Add(ICrawlerAsync item)
        {
            item.ThrowIfNull(nameof(item));
            if (!_crawlersAsync.Contains(item))
            {
                _crawlersAsync.Add(item);
            }
        }

        public bool Remove(ICrawlerAsync item)
        {
            item.ThrowIfNull(nameof(item));
            return _crawlersAsync.Remove(item);
        }

        #endregion

        #region IDisposable Implementation

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            _cancellationTokenSource.Dispose();

            _crawlersAsync.ForEach(crawlerAsync => crawlerAsync.Dispose());
        }

        #endregion

        public async Task<bool> CollectAllResponses(ISourceBlock<string> entitiesQueue,
            IDictionary<Type, BufferBlock<BasicInfo>> rawDataQueues, DataflowBlockOptions options)
        {
            var producers = new List<Task<bool>>(_crawlersAsync.Count);
            var consumers = new List<ITargetBlock<string>>(_crawlersAsync.Count);

            foreach (ICrawlerAsync crawlerAsync in _crawlersAsync)
            {
                var consumer = new BufferBlock<string>(options);
                var rawDataQueue = new BufferBlock<BasicInfo>(options);

                rawDataQueues.Add(crawlerAsync.TypeId, rawDataQueue);

                Task<bool> producerTask = crawlerAsync
                    .GetResponse(consumer, rawDataQueue, _outputResults)
                    .CancelIfFaulted(_cancellationTokenSource);

                producers.Add(producerTask);
                consumers.Add(consumer);
            }

            Task<ResultOrException<bool>[]> statusesTask =
                TaskHelper.WhenAllResultsOrExceptions(producers);

            Task consumersTasks = Task.WhenAll(consumers.Select(consumer => consumer.Completion));

            Task splitQueueTask = SplitQueue(entitiesQueue, consumers,
                                             _cancellationTokenSource.Token);

            try
            {
                await Task.WhenAll(splitQueueTask, consumersTasks, statusesTask);

                (IReadOnlyList<bool> statuses, IReadOnlyList<Exception> taskExceptions) =
                    statusesTask.Result.UnwrapResultsOrExceptions();

                CheckExceptions(taskExceptions);

                if (statuses.Any() && statuses.All(r => r))
                {
                    _logger.Info("Crawlers have finished work.");
                    return true;
                }
            }
            catch (TaskCanceledException)
            {
                if (statusesTask.IsCompleted)
                {
                    (IReadOnlyList<bool> statuses, IReadOnlyList<Exception> taskExceptions) =
                        statusesTask.Result.UnwrapResultsOrExceptions();

                    CheckExceptions(taskExceptions);
                }

                throw;
            }
            finally
            {
                // Need to release queues in any cases.
                rawDataQueues.Values.MarkAsCompletedSafe();
            }

            _logger.Info("Crawlers have not received some data.");
            return false;
        }

        private async Task SplitQueue(ISourceBlock<string> entitiesQueue,
            IReadOnlyList<ITargetBlock<string>> consumers, CancellationToken cancellationToken)
        {
            // No exception should be thrown before try-finally block.
            try
            {
                while (await entitiesQueue.OutputAvailableAsync(cancellationToken))
                {
                    string entity = await entitiesQueue.ReceiveAsync(cancellationToken);

                    if (_outputResults)
                    {
                        GlobalMessageHandler.OutputMessage(
                            $"Got {entity} and transmitted to crawling."
                        );
                    }

                    await Task.WhenAll(
                        consumers.Select(consumer => consumer.SendAsync(entity, cancellationToken))
                    );
                }
            }
            finally
            {
                // No exception should be thrown in finally block before consumer queues would be
                // marked as completed.
                consumers.MarkAsCompletedSafe();
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
    }
}
