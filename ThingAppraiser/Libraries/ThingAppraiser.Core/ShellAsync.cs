using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Xml.Linq;
using ThingAppraiser.Communication;
using ThingAppraiser.Core.ShellBuilders;
using ThingAppraiser.DataPipeline;
using ThingAppraiser.Extensions;
using ThingAppraiser.Logging;
using ThingAppraiser.Models.Internal;

namespace ThingAppraiser.Core
{
    public sealed class ShellAsync : IDisposable
    {
        private static readonly ILogger _logger = LoggerFactory.CreateLoggerFor<ShellAsync>();

        private readonly int _boundedCapacity;

        private readonly DataflowBlockOptions _dataFlowOptions;

        public IO.Input.InputManagerAsync InputManagerAsync { get; }

        public Crawlers.CrawlersManagerAsync CrawlersManagerAsync { get; }

        public Appraisers.AppraisersManagerAsync AppraisersManagerAsync { get; }

        public IO.Output.OutputManagerAsync OutputManagerAsync { get; }

        private readonly CancellationTokenSource _cancellationTokenSource =
            new CancellationTokenSource();

        private bool _disposed;


        public ShellAsync(
            IO.Input.InputManagerAsync inputManagerAsync,
            Crawlers.CrawlersManagerAsync crawlersManagerAsync,
            Appraisers.AppraisersManagerAsync appraisersManagerAsync,
            IO.Output.OutputManagerAsync outputManagerAsync,
            int boundedCapacity)
        {
            InputManagerAsync = inputManagerAsync.ThrowIfNull(nameof(inputManagerAsync));
            CrawlersManagerAsync = crawlersManagerAsync.ThrowIfNull(nameof(crawlersManagerAsync));
            AppraisersManagerAsync =
                appraisersManagerAsync.ThrowIfNull(nameof(appraisersManagerAsync));
            OutputManagerAsync = outputManagerAsync.ThrowIfNull(nameof(outputManagerAsync));

            _boundedCapacity = boundedCapacity;
            _dataFlowOptions = new DataflowBlockOptions
            { 
                BoundedCapacity = _boundedCapacity,
                CancellationToken = _cancellationTokenSource.Token
            };
        }

        public static ShellAsyncBuilderDirector CreateBuilderDirector(XDocument configuration)
        {
            return new ShellAsyncBuilderDirector(new ShellAsyncBuilderFromXDocument(configuration));
        }

        private async Task<ServiceStatus> SaveResults(OutputtersFlow outputtersFlow)
        {
            try
            {
                bool status = await OutputManagerAsync.SaveResults(
                    outputtersFlow, storageName: string.Empty
                );
                if (status)
                {
                    GlobalMessageHandler.OutputMessage("Ratings was saved successfully.");
                    return ServiceStatus.Ok;
                }

                GlobalMessageHandler.OutputMessage("Ratings wasn't saved.");
                return ServiceStatus.OutputUnsaved;
            }
            catch (Exception ex)
            {
                _cancellationTokenSource.Cancel();

                _logger.Error(ex, "Exception occured during output work.");
                return ServiceStatus.OutputError;
            }
        }

        public async Task<ServiceStatus> Run(string storageName)
        {
            GlobalMessageHandler.OutputMessage("Shell started work.");
            _logger.Info("Shell started work.");

            // Input component work.
            var inputtersFlow = InputManagerAsync.GetNames(storageName);

            // Crawlers component work.
            var crawlersFlow = CrawlersManagerAsync.CollectAllResponses();

            // Appraisers component work.
            var appraisersFlow = AppraisersManagerAsync.GetAllRatings();

            // Output component work.
            var outputtersFlow = OutputManagerAsync.CreateFlow(storageName: string.Empty);

            // Constructing pipeline.
            inputtersFlow.LinkTo(crawlersFlow);
            crawlersFlow.LinkTo(appraisersFlow);
            appraisersFlow.LinkTo(outputtersFlow);

            // Start processing data.
            await inputtersFlow.ProcessAsync(new[] { storageName });

            // Wait for the last block in the pipeline to process all messages.
            Task completionTask = outputtersFlow.CompletionTask;

            Task<ServiceStatus> statusTask = SaveResults(outputtersFlow);

            await Task.WhenAll(completionTask, statusTask);

            ServiceStatus status = statusTask.Result;
            // TODO: if there are error statuses need to create aggregate status which contains
            // more details then simple ServiceStatus.Error value.
            if (status != ServiceStatus.Ok)
            {
                GlobalMessageHandler.OutputMessage(
                    "Shell got error status during data processing."
                );
                _logger.Info("Shell got error status during data processing.");
                return ServiceStatus.Error;
            }

            _logger.Info("Shell finished work successfully.");
            GlobalMessageHandler.OutputMessage("Shell finished work successfully.");
            return ServiceStatus.Ok;
        }

        #region IDisposable Implementation

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            _cancellationTokenSource.Dispose();

            CrawlersManagerAsync.Dispose();
            AppraisersManagerAsync.Dispose();
        }

        #endregion
    }
}
