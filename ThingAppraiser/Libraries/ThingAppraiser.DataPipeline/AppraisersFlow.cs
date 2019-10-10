using System;
using System.Collections.Generic;
using System.Threading.Tasks.Dataflow;
using Gridsum.DataflowEx;
using ThingAppraiser.Extensions;
using ThingAppraiser.Models.Data;
using ThingAppraiser.Models.Internal;

namespace ThingAppraiser.DataPipeline
{
    public sealed class AppraisersFlow : Dataflow<BasicInfo, RatingDataContainer>
    {
        private readonly Dataflow<BasicInfo, BasicInfo> _inputConsumer;

        private readonly Dataflow<RatingDataContainer, RatingDataContainer> _resultConsumer;

        public override ITargetBlock<BasicInfo> InputBlock =>
            _inputConsumer.InputBlock;

        public override ISourceBlock<RatingDataContainer> OutputBlock =>
            _resultConsumer.OutputBlock;


        public AppraisersFlow(IEnumerable<Appraiser> appraisers)
            : base(DataflowOptions.Default)
        {
            appraisers.ThrowIfNull(nameof(appraisers));

            _inputConsumer = new DataBroadcaster<BasicInfo>(crawlersData =>
            {
                Console.WriteLine($"Broadcasts all crawlers data.");
                return crawlersData;
            }, DataflowOptions.Default);

            _resultConsumer = new TransformBlock<RatingDataContainer, RatingDataContainer>(
                appraisedData => appraisedData
            ).ToDataflow(DataflowOptions.Default);

            var usedTypes = new Dictionary<Type, DataBroadcaster<BasicInfo>>();
            foreach (var appraiser in appraisers)
            {
                if (!usedTypes.TryGetValue(appraiser.DataType, out var broadcaster))
                {
                    broadcaster = new DataBroadcaster<BasicInfo>(crawlersData =>
                    {
                        Console.WriteLine($"Broadcasts specified data of type {appraiser.DataType.Name}.");
                        return crawlersData;
                    }, DataflowOptions.Default);

                    usedTypes.Add(appraiser.DataType, broadcaster);
                    _inputConsumer.TransformAndLink(
                        broadcaster,
                        info => info,
                        info => info.GetType().IsAssignableFrom(appraiser.DataType)
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
