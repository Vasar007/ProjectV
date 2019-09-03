using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using ThingAppraiser.Communication;
using ThingAppraiser.Logging;
using ThingAppraiser.Models.Data;
using ThingAppraiser.Models.Internal;

namespace ThingAppraiser.Appraisers
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

        public async Task<bool> GetAllRatings(
            IDictionary<Type, BufferBlock<BasicInfo>> rawDataQueues,
            IList<BufferBlock<RatingDataContainer>> appraisedDataQueues,
            DataflowBlockOptions options)
        {
            var producers = new List<Task<bool>>(rawDataQueues.Count);
            var consumersTasks = new List<Task>(rawDataQueues.Count);
            var splitQueueTasks = new List<Task>(rawDataQueues.Count);

            foreach (KeyValuePair<Type, BufferBlock<BasicInfo>> keyValue in rawDataQueues)
            {
                if (!_appraisersAsync.TryGetValue(keyValue.Key, out IList<IAppraiserAsync> values))
                {
                    string message = $"Type {keyValue.Key} wasn't used to appraise!";
                    _logger.Info(message);
                    GlobalMessageHandler.OutputMessage(message);
                    continue;
                }

                var consumers = new List<ITargetBlock<BasicInfo>>(values.Count);
                foreach (IAppraiserAsync appraiserAsync in values)
                {
                    var consumer = new BufferBlock<BasicInfo>(options);
                    var appraisedDataQueue = new BufferBlock<RatingDataContainer>(options);

                    appraisedDataQueues.Add(appraisedDataQueue);
                    producers.Add(
                        appraiserAsync.GetRatings(consumer, appraisedDataQueue, _outputResults)
                    );
                    consumers.Add(consumer);
                }

                consumersTasks.Add(Task.WhenAll(consumers.Select(consumer => consumer.Completion)));
                splitQueueTasks.Add(SplitQueue(keyValue.Value, consumers));
            }

            Task<bool[]> statusesTask = Task.WhenAll(producers);
            Task consumersFinalTask = Task.WhenAll(consumersTasks);
            Task splitQueueFinalTask = Task.WhenAll(splitQueueTasks);

            await Task.WhenAll(splitQueueFinalTask, consumersFinalTask, statusesTask);

            IReadOnlyList<bool> statuses = await statusesTask;
            foreach (BufferBlock<RatingDataContainer> appraisedQueue in appraisedDataQueues)
            {
                appraisedQueue.Complete();
            }

            if (!statuses.IsNullOrEmpty() && statuses.All(r => r))
            {
                _logger.Info("Appraisers have finished work.");
                return true;
            }

            _logger.Info("Appraisers have not processed any data.");
            return false;
        }

        private async Task SplitQueue(ISourceBlock<BasicInfo> rawDataQueue,
            IReadOnlyList<ITargetBlock<BasicInfo>> consumers)
        {
            while (await rawDataQueue.OutputAvailableAsync())
            {
                BasicInfo entity = await rawDataQueue.ReceiveAsync();

                if (_outputResults)
                {
                    GlobalMessageHandler.OutputMessage(
                        $"Got {entity.Title} and transmitted to appraising."
                    );
                }

                await Task.WhenAll(
                    consumers.Select(async consumer => await consumer.SendAsync(entity))
                );
            }

            foreach (ITargetBlock<BasicInfo> consumer in consumers)
            {
                consumer.Complete();
            }
        }
    }
}
