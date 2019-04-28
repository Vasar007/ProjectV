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
    public sealed class CAppraisersManagerAsync : IManager<CAppraiserAsync>
    {
        private static readonly CLoggerAbstraction s_logger =
            CLoggerAbstraction.CreateLoggerInstanceFor<CAppraisersManagerAsync>();

        private readonly Dictionary<Type, List<CAppraiserAsync>> _appraisersAsync =
            new Dictionary<Type, List<CAppraiserAsync>>();

        private readonly Boolean _outputResults;


        public CAppraisersManagerAsync(Boolean outputResults)
        {
            _outputResults = outputResults;
        }

        #region IManager<CAppraiserAsync> Implementation

        public void Add(CAppraiserAsync item)
        {
            item.ThrowIfNull(nameof(item));

            if (_appraisersAsync.TryGetValue(item.TypeID, out List<CAppraiserAsync> list))
            {
                if (!list.Contains(item))
                {
                    list.Add(item);
                }
            }
            else
            {
                _appraisersAsync.Add(item.TypeID, new List<CAppraiserAsync> { item });
            }
        }

        public Boolean Remove(CAppraiserAsync item)
        {
            item.ThrowIfNull(nameof(item));
            return _appraisersAsync.Remove(item.TypeID);
        }

        #endregion

        public async Task<Boolean> GetAllRatings(
            Dictionary<Type, BufferBlock<CBasicInfo>> entitiesInfoQueues,
            Dictionary<Type, BufferBlock<CRatingDataContainer>> entitiesRatingQueues,
            DataflowBlockOptions options)
        {
            List<Task<Boolean>> producers = new List<Task<Boolean>>();
            foreach (KeyValuePair<Type, BufferBlock<CBasicInfo>> keyValue in entitiesInfoQueues)
            {
                if (!_appraisersAsync.TryGetValue(keyValue.Key, out List<CAppraiserAsync> values))
                {
                    String message = $"Type {keyValue.Key} wasn't used to appraise!";
                    s_logger.Info(message);
                    SGlobalMessageHandler.OutputMessage(message);
                    continue;
                }

                var entitiesRatingQueue = new BufferBlock<CRatingDataContainer>(options);
                entitiesRatingQueues.Add(keyValue.Key, entitiesRatingQueue);
                producers.AddRange(values.Select(
                    appraiserAsync => appraiserAsync.GetRatings(keyValue.Value, entitiesRatingQueue,
                                                                _outputResults))
                );
            }

            Boolean[] statuses = await Task.WhenAll(producers);
            foreach (BufferBlock<CRatingDataContainer> entitiesRatingQueue in 
                     entitiesRatingQueues.Values)
            {
                entitiesRatingQueue.Complete();
            }

            if (!statuses.IsNullOrEmpty() && statuses.All(r => r))
            {
                s_logger.Info("Appraisers have finished work.");
                return true;
            }

            s_logger.Info("Appraisers have not processed any data.");
            return false;
        }
    }
}
