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
using ThingAppraiser.Models.Internal;

namespace ThingAppraiser.Appraisers
{
    public sealed class AppraisersManagerAsync : IManager<IAppraiserAsync>, IDisposable
    {
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor<AppraisersManagerAsync>();

        private readonly Dictionary<Type, IList<IAppraiserAsync>> _appraisersAsync =
            new Dictionary<Type, IList<IAppraiserAsync>>();

        private readonly bool _outputResults;

        private readonly CancellationTokenSource _cancellationTokenSource =
            new CancellationTokenSource();

        private bool _disposed;


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

        #region IDisposable Implementation

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            _cancellationTokenSource.Dispose();
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
                    string message = $"Type {keyValue.Key} was not used to appraise!";
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

                    Task<bool> producerTask = appraiserAsync
                        .GetRatings(consumer, appraisedDataQueue, _outputResults)
                        .CancelIfFaulted(_cancellationTokenSource);

                    producers.Add(producerTask);
                    consumers.Add(consumer);
                }

                consumersTasks.AddRange(consumers.Select(consumer => consumer.Completion));

                Task splitQueueTask = SplitQueue(keyValue.Value, consumers,
                                                 _cancellationTokenSource.Token);
                splitQueueTasks.Add(splitQueueTask);
            }

            Task<ResultOrException<bool>[]> statusesTask =
                TaskHelper.WhenAllResultsOrExceptions(producers);

            Task consumersFinalTask = Task.WhenAll(consumersTasks);
            Task splitQueueFinalTask = Task.WhenAll(splitQueueTasks);

            try
            {
                await Task.WhenAll(splitQueueFinalTask, consumersFinalTask, statusesTask);

                (IReadOnlyList<bool> statuses, IReadOnlyList<Exception> taskExceptions) =
                    statusesTask.Result.UnwrapResultsOrExceptions();

                CheckExceptions(taskExceptions);

                if (statuses.Any() && statuses.All(r => r))
                {
                    _logger.Info("Appraisers have finished work.");
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
                appraisedDataQueues.MarkAsCompletedSafe();
            }

            _logger.Info("Appraisers have not processed some data.");
            return false;
        }

        private async Task SplitQueue(ISourceBlock<BasicInfo> rawDataQueue,
            IReadOnlyList<ITargetBlock<BasicInfo>> consumers, CancellationToken cancellationToken)
        {
            // No exception should be thrown before try-finally block.
            try
            {
                while (await rawDataQueue.OutputAvailableAsync(cancellationToken))
                {
                    BasicInfo entity = await rawDataQueue.ReceiveAsync(cancellationToken);

                    if (_outputResults)
                    {
                        GlobalMessageHandler.OutputMessage(
                            $"Got {entity.Title} and transmitted to appraising."
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
                throw new Exception($"One of the appraisers failed.", taskExceptions.Single());
            }

            throw new AggregateException(
                $"Some appraisers failed. Exceptions number: {taskExceptions.Count.ToString()}.",
                taskExceptions
            );
        }
    }
}
