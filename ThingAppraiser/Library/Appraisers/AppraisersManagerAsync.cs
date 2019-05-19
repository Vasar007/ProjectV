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
            IDictionary<Type, BufferBlock<BasicInfo>> entitiesInfoQueues,
            IDictionary<Type, BufferBlock<RatingDataContainer>> entitiesRatingQueues,
            DataflowBlockOptions options)
        {
            var producers = new List<Task<bool>>(entitiesInfoQueues.Count);
            foreach (KeyValuePair<Type, BufferBlock<BasicInfo>> keyValue in entitiesInfoQueues)
            {
                if (!_appraisersAsync.TryGetValue(keyValue.Key, out List<AppraiserAsync> values))
                {
                    string message = $"Type {keyValue.Key} wasn't used to appraise!";
                    _logger.Info(message);
                    GlobalMessageHandler.OutputMessage(message);
                    continue;
                }

                // FIXME: need to split queue to several queues which would be unique for every 
                // appraiser.
                var entitiesRatingQueue = new BufferBlock<RatingDataContainer>(options);
                entitiesRatingQueues.Add(keyValue.Key, entitiesRatingQueue);
                producers.AddRange(values.Select(
                    appraiserAsync => appraiserAsync.GetRatings(keyValue.Value, entitiesRatingQueue,
                                                                _outputResults))
                );
            }

            bool[] statuses = await Task.WhenAll(producers);
            foreach (BufferBlock<RatingDataContainer> entitiesRatingQueue in 
                     entitiesRatingQueues.Values)
            {
                entitiesRatingQueue.Complete();
            }

            if (!statuses.IsNullOrEmpty() && statuses.All(r => r))
            {
                _logger.Info("Appraisers have finished work.");
                return true;
            }

            _logger.Info("Appraisers have not processed any data.");
            return false;
        }
    }
}
