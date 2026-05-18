using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks.Dataflow;
using Acolyte.Assertions;
using Acolyte.Linq;
using Gridsum.DataflowEx;
using ProjectV.Models.Data;

namespace ProjectV.DataPipeline
{
    public sealed class CrawlersFlow : Dataflow<string, BasicInfo>
    {
        private readonly Dataflow<string, string> _inputBroadcaster;

        private readonly Dataflow<BasicInfo, BasicInfo> _resultConsumer;

        public override ITargetBlock<string> InputBlock => _inputBroadcaster.InputBlock;

        public override ISourceBlock<BasicInfo> OutputBlock => _resultConsumer.OutputBlock;


        public CrawlersFlow(IEnumerable<Func<string, IAsyncEnumerable<BasicInfo>>> crawlers)
            : base(DataflowOptions.Default)
        {
            crawlers.ThrowIfNull(nameof(crawlers));

            _inputBroadcaster = new DataBroadcaster<string>(
                filteredData => filteredData, DataflowOptions.Default
            );

            _resultConsumer = new TransformBlock<BasicInfo, BasicInfo>(
                crawlersData => crawlersData
            ).ToDataflow(DataflowOptions.Default);

            InitFlow(crawlers);
        }

        private void InitFlow(IEnumerable<Func<string, IAsyncEnumerable<BasicInfo>>> crawlers)
        {
            var crawlerFlows = crawlers.Select(crawler =>
                new TransformManyBlock<string, BasicInfo>(
                    async entity => await crawler(entity).AsEnumerableAsync().LogIfErrorOccured()
                ).ToDataflow(DataflowOptions.Default)
            );

            foreach (Dataflow<string, BasicInfo> crawlerFlow in crawlerFlows)
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
