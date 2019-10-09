using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks.Dataflow;
using Gridsum.DataflowEx;
using ThingAppraiser.Extensions;
using ThingAppraiser.Models.Data;

namespace ThingAppraiser.DataPipeline
{
    public sealed class CrawlersFlow : Dataflow<IReadOnlyList<string>, IReadOnlyList<BasicInfo>>
    {
        private readonly Dataflow<IReadOnlyList<string>, IReadOnlyList<string>> _inputBroadcaster;

        private readonly Dataflow<IReadOnlyList<BasicInfo>, IReadOnlyList<BasicInfo>> _resultConsumer;

        public override ITargetBlock<IReadOnlyList<string>> InputBlock =>
            _inputBroadcaster.InputBlock;

        public override ISourceBlock<IReadOnlyList<BasicInfo>> OutputBlock =>
            _resultConsumer.OutputBlock;


        public CrawlersFlow(IEnumerable<Func<IReadOnlyList<string>, IReadOnlyList<BasicInfo>>> crawlers)
            : base(DataflowOptions.Default)
        {
            crawlers.ThrowIfNull(nameof(crawlers));

            _inputBroadcaster = new DataBroadcaster<IReadOnlyList<string>>(filteredData =>
            {
                Console.WriteLine($"Broadcasts all filtered inputters data. {filteredData.Count.ToString()}");
                return filteredData;
            }, DataflowOptions.Default);

            _resultConsumer = new TransformBlock<IReadOnlyList<BasicInfo>, IReadOnlyList<BasicInfo>>(
                crawlersData => crawlersData
            ).ToDataflow(DataflowOptions.Default);

            var crawlerFlows = crawlers
                .Select(crawler => DataflowUtils.FromDelegate(crawler, DataflowOptions.Default));
            foreach (var crawlerFlow in crawlerFlows)
            {
                _inputBroadcaster.LinkTo(crawlerFlow);
                crawlerFlow.LinkTo(_resultConsumer);

                _resultConsumer.RegisterDependency(crawlerFlow);
                RegisterChild(crawlerFlow);
            }

            RegisterChild(_inputBroadcaster);
            RegisterChild(_resultConsumer);
        }
    }
}
