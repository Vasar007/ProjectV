using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using ThingAppraiser.Communication;
using ThingAppraiser.Data;
using ThingAppraiser.Logging;

namespace ThingAppraiser.Appraisers
{
    public sealed class AppraisersManagerAsync : IManager<AppraiserAsync>
    {
        private static readonly LoggerAbstraction _logger =
            LoggerAbstraction.CreateLoggerInstanceFor<AppraisersManagerAsync>();

        private readonly Dictionary<Type, List<AppraiserAsync>> _appraisersAsync =
            new Dictionary<Type, List<AppraiserAsync>>();

        private readonly bool _outputResults;


        public AppraisersManagerAsync(bool outputResults)
        {
            _outputResults = outputResults;
        }

        #region IManager<AppraiserAsync> Implementation

        public void Add(AppraiserAsync item)
        {
            item.ThrowIfNull(nameof(item));

            if (_appraisersAsync.TryGetValue(item.TypeId, out List<AppraiserAsync> list))
            {
                if (!list.Contains(item))
                {
                    list.Add(item);
                }
            }
            else
            {
                _appraisersAsync.Add(item.TypeId, new List<AppraiserAsync> { item });
            }
        }

        public bool Remove(AppraiserAsync item)
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
                if (!_appraisersAsync.TryGetValue(keyValue.Key, out List<AppraiserAsync> values))
                {
                    string message = $"Type {keyValue.Key} wasn't used to appraise!";
                    _logger.Info(message);
                    GlobalMessageHandler.OutputMessage(message);
                    continue;
                }

                var consumers = new List<BufferBlock<BasicInfo>>(values.Count);
                foreach (AppraiserAsync appraiserAsync in values)
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

            bool[] statuses = await statusesTask;
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

        private async Task SplitQueue(BufferBlock<BasicInfo> rawDataQueue,
            IList<BufferBlock<BasicInfo>> consumers)
        {
            while (await rawDataQueue.OutputAvailableAsync())
            {
                BasicInfo entity = await rawDataQueue.ReceiveAsync();

                if (_outputResults)
                {
                    GlobalMessageHandler.OutputMessage($"Got {entity}");
                }

                await Task.WhenAll(
                    consumers.Select(async consumer => await consumer.SendAsync(entity))
                );
            }

            foreach (BufferBlock<BasicInfo> consumer in consumers)
            {
                consumer.Complete();
            }
        }
    }
}
