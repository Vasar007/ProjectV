using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Xml.Linq;
using ThingAppraiser.Communication;
using ThingAppraiser.Core.ShellBuilders;
using ThingAppraiser.Extensions;
using ThingAppraiser.Logging;
using ThingAppraiser.Models.Data;
using ThingAppraiser.Models.Internal;

namespace ThingAppraiser.Core
{
    public sealed class ShellAsync
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

        private async Task<ServiceStatus> GetThingNames(ITargetBlock<string> queue,
            string storageName)
        {
            try
            {
                bool status = await InputManagerAsync.GetNames(queue, storageName);
                if (status)
                {
                    GlobalMessageHandler.OutputMessage("Things were successfully gotten.");
                    return ServiceStatus.Ok;
                }

                GlobalMessageHandler.OutputMessage($"No Things were found in \"{storageName}\".");
                return ServiceStatus.Nothing;
            }
            catch (Exception ex)
            {
                _cancellationTokenSource.Cancel();

                _logger.Error(ex, "Exception occured during input work.");
                return ServiceStatus.InputError;
            }
        }

        private async Task<ServiceStatus> RequestData(ISourceBlock<string> entitiesQueue,
            IDictionary<Type, BufferBlock<BasicInfo>> rawDataQueues)
        {
            try
            {
                bool status = await CrawlersManagerAsync.CollectAllResponses(
                    entitiesQueue, rawDataQueues, _dataFlowOptions
                );
                if (status)
                {
                    GlobalMessageHandler.OutputMessage(
                        "Crawlers have received responses from services."
                    );
                    return ServiceStatus.Ok;
                }

                GlobalMessageHandler.OutputMessage(
                    "Crawlers have not received responses from services. Result is empty."
                );
                return ServiceStatus.Nothing;
            }
            catch (Exception ex)
            {
                _cancellationTokenSource.Cancel();

                _logger.Error(ex, "Exception occured during collecting data.");
                return ServiceStatus.RequestError;
            }
        }

        private async Task<ServiceStatus> AppraiseThings(
            IDictionary<Type, BufferBlock<BasicInfo>> rawDataQueues,
            IList<BufferBlock<RatingDataContainer>> appraisedDataQueues)
        {
            try
            {
                bool status = await AppraisersManagerAsync.GetAllRatings(
                    rawDataQueues, appraisedDataQueues, _dataFlowOptions
                );
                if (status)
                {
                    GlobalMessageHandler.OutputMessage(
                        "Appraisers have calculated ratings successfully."
                    );
                    return ServiceStatus.Ok;
                }

                GlobalMessageHandler.OutputMessage(
                    "Appraisers have not calculated ratings. Result is empty."
                );
                return ServiceStatus.Nothing;
            }
            catch (Exception ex)
            {
                _cancellationTokenSource.Cancel();

                _logger.Error(ex, "Exception occured during appraising work.");
                return ServiceStatus.AppraiseError;
            }
        }

        private async Task<ServiceStatus> SaveResults(
            IReadOnlyList<ISourceBlock<RatingDataContainer>> appraisedDataQueues)
        {
            try
            {
                bool status = await OutputManagerAsync.SaveResults(appraisedDataQueues,
                                                                   string.Empty);
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

            var inputQueue = new BufferBlock<string>(_dataFlowOptions);

            var rawDataQueues = new Dictionary<Type, BufferBlock<BasicInfo>>();

            var appraisedDataQueues = new List<BufferBlock<RatingDataContainer>>();

            // Input component work.
            Task<ServiceStatus> inputStatus = GetThingNames(inputQueue, storageName);

            // Crawlers component work.
            Task<ServiceStatus> crawlersStatus = RequestData(inputQueue, rawDataQueues);

            // Appraisers component work.
            Task<ServiceStatus> appraisersStatus = AppraiseThings(rawDataQueues,
                                                                  appraisedDataQueues);

            // Output component work.
            Task<ServiceStatus> outputStatus = SaveResults(appraisedDataQueues);

            Task<ServiceStatus[]> statusesTask = Task.WhenAll(inputStatus, crawlersStatus,
                                                              appraisersStatus, outputStatus);

            Task rawDataQueuesTasks = Task.WhenAll(
                rawDataQueues.Values.Select(bufferBlock => bufferBlock.Completion)
            );
            Task appraisedDataQueuesTasks = Task.WhenAll(
                appraisedDataQueues.Select(bufferBlock => bufferBlock.Completion)
            );

            await Task.WhenAll(statusesTask, inputQueue.Completion, rawDataQueuesTasks,
                               appraisedDataQueuesTasks);

            // FIX ME: if there are error statuses need to create aggregate status which contains
            // more details then simple ServiceStatus.Error value.
            ServiceStatus[] statuses = await statusesTask;
            if (statuses.Any(status => status != ServiceStatus.Ok))
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
        }

        #endregion
    }
}
