using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks.Dataflow;
using Acolyte.Assertions;
using Gridsum.DataflowEx;
using ThingAppraiser.Models.Internal;

namespace ThingAppraiser.DataPipeline
{
    public sealed class OutputtersFlow : Dataflow<RatingDataContainer>
    {
        private readonly ConcurrentBag<RatingDataContainer> _resultList;

        private readonly Dataflow<RatingDataContainer, RatingDataContainer> _inputConsumer;

        public override ITargetBlock<RatingDataContainer> InputBlock => _inputConsumer.InputBlock;

        public IEnumerable<RatingDataContainer> Results => _resultList;


        public OutputtersFlow(IEnumerable<Action<RatingDataContainer>> outputters)
            : base(DataflowOptions.Default)
        {
            outputters.ThrowIfNull(nameof(outputters));

            _resultList = new ConcurrentBag<RatingDataContainer>();

            _inputConsumer = new DataBroadcaster<RatingDataContainer>(appraisersData =>
            {
                _resultList.Add(appraisersData);
                return appraisersData;
            }, DataflowOptions.Default);

            InitFlow(outputters);
        }

        public void Clear()
        {
            _resultList.Clear();
        }

        private void InitFlow(IEnumerable<Action<RatingDataContainer>> outputters)
        {
            var outputterFlows = outputters.Select(outputters =>
                DataflowUtils.FromDelegate(outputters, DataflowOptions.Default)
            );
            foreach (Dataflow<RatingDataContainer> outputterFlow in outputterFlows)
            {
                _inputConsumer.LinkTo(outputterFlow);

                RegisterChild(outputterFlow);
            }

            RegisterChild(_inputConsumer);
        }
    }
}
