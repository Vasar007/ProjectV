using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks.Dataflow;
using Gridsum.DataflowEx;
using ThingAppraiser.Extensions;
using ThingAppraiser.Models.Data;

namespace ThingAppraiser.DataPipeline
{
    public sealed class AppraisersFlow : Dataflow<IReadOnlyList<BasicInfo>, IReadOnlyList<string>>
    {
        private readonly Dataflow<IReadOnlyList<BasicInfo>, IReadOnlyList<BasicInfo>> _inputConsumer;

        private readonly Dataflow<IReadOnlyList<string>, IReadOnlyList<string>> _resultConsumer;

        public override ITargetBlock<IReadOnlyList<BasicInfo>> InputBlock =>
            _inputConsumer.InputBlock;

        public override ISourceBlock<IReadOnlyList<string>> OutputBlock =>
            _resultConsumer.OutputBlock;


        public AppraisersFlow(IEnumerable<Appraiser> appraisers)
            : base(DataflowOptions.Default)
        {
            appraisers.ThrowIfNull(nameof(appraisers));

            _inputConsumer = new DataBroadcaster<IReadOnlyList<BasicInfo>>(crawlersData =>
            {
                Console.WriteLine($"Broadcasts all crawlers data. {crawlersData.Count.ToString()}");
                return crawlersData;
            }, DataflowOptions.Default);

            _resultConsumer = new TransformBlock<IReadOnlyList<string>, IReadOnlyList<string>>(
                appraisedData => appraisedData
            ).ToDataflow(DataflowOptions.Default);

            var usedTypes = new Dictionary<Type, DataBroadcaster<IReadOnlyList<BasicInfo>>>();
            foreach (var appraiser in appraisers)
            {
                if (!usedTypes.TryGetValue(appraiser.DataType, out var broadcaster))
                {
                    broadcaster = new DataBroadcaster<IReadOnlyList<BasicInfo>>(crawlersData =>
                    {
                        Console.WriteLine($"Broadcasts specified data of type {appraiser.DataType.Name}. {crawlersData.Count.ToString()}");
                        return crawlersData;
                    }, DataflowOptions.Default);

                    usedTypes.Add(appraiser.DataType, broadcaster);
                    _inputConsumer.TransformAndLink(
                        broadcaster,
                        l => l,
                        l => l.All(d => d.GetType().IsAssignableFrom(appraiser.DataType))
                    );
                    RegisterChild(broadcaster);
                }
                var appraiserFlow = DataflowUtils.FromDelegate(appraiser.Func, DataflowOptions.Default);
                broadcaster.LinkTo(appraiserFlow);
                appraiserFlow.LinkTo(_resultConsumer);

                _resultConsumer.RegisterDependency(appraiserFlow);
                RegisterChild(appraiserFlow);
            }

            RegisterChild(_inputConsumer);
            RegisterChild(_resultConsumer);
        }
    }
}
