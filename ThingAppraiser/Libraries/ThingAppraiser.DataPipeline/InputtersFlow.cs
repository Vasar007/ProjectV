using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks.Dataflow;
using Gridsum.DataflowEx;
using ThingAppraiser.Extensions;

namespace ThingAppraiser.DataPipeline
{
    public sealed class InputtersFlow : Dataflow<string, string>
    {
        private readonly ConcurrentDictionary<string, byte> _filteringSet;

        private readonly Dataflow<string, string> _inputBroadcaster;

        private readonly Dataflow<string, string> _resultTransformer;

        public override ITargetBlock<string> InputBlock => _inputBroadcaster.InputBlock;

        public override ISourceBlock<string> OutputBlock =>
            _resultTransformer.OutputBlock;


        public InputtersFlow(IEnumerable<Func<string, IEnumerable<string>>> inputters)
            : base(DataflowOptions.Default)
        {
            inputters.ThrowIfNull(nameof(inputters));

            _filteringSet = new ConcurrentDictionary<string, byte>();

            _inputBroadcaster = new DataBroadcaster<string>(input =>
            {
                Console.WriteLine("Broadcasts input to further blocks.");
                return input;
            }, DataflowOptions.Default);

            _resultTransformer = DataflowUtils.FromDelegate<string, string>(
                inputtersData => inputtersData, DataflowOptions.Default
            );

            var inputFlows = inputters
                .Select(inputter => DataflowUtils.FromDelegate(inputter, DataflowOptions.Default));
            foreach (var inputFlow in inputFlows)
            {
                _inputBroadcaster.LinkTo(inputFlow);
                inputFlow.LinkTo(_resultTransformer, FilterInputData);

                _resultTransformer.RegisterDependency(inputFlow);
                RegisterChild(inputFlow);
            }

            RegisterChild(_inputBroadcaster);
            RegisterChild(_resultTransformer);
        }

        protected override void CleanUp(Exception dataflowException)
        {
            _filteringSet.Clear();

            base.CleanUp(dataflowException);
        }

        private bool FilterInputData(string inputtersData)
        {
            return inputtersData.Length > 2 &&
                   _filteringSet.TryAdd(inputtersData, default);
        }
    }
}
