using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using ThingAppraiser.Communication;
using ThingAppraiser.Logging;

namespace ThingAppraiser.Core
{
    public sealed class CShellAsync
    {
        private static readonly CLoggerAbstraction s_logger =
            CLoggerAbstraction.CreateLoggerInstanceFor<CShellAsync>();

        private readonly Int32 _boundedCapacity;

        private readonly DataflowBlockOptions _dataFlowOptions;

        public IO.Input.CInputManagerAsync InputManagerAsync { get; }

        public Crawlers.CCrawlersManagerAsync CrawlersManagerAsync { get; }

        public Appraisers.CAppraisersManagerAsync AppraisersManagerAsync { get; }

        public IO.Output.COutputManagerAsync OutputManagerAsync { get; }


        public CShellAsync(
            IO.Input.CInputManagerAsync inputManagerAsync,
            Crawlers.CCrawlersManagerAsync crawlersManagerAsync,
            Appraisers.CAppraisersManagerAsync appraisersManagerAsync,
            IO.Output.COutputManagerAsync outputManagerAsync,
            Int32 boundedCapacity)
        {
            InputManagerAsync = inputManagerAsync.ThrowIfNull(nameof(inputManagerAsync));
            CrawlersManagerAsync = crawlersManagerAsync.ThrowIfNull(nameof(crawlersManagerAsync));
            AppraisersManagerAsync = 
                appraisersManagerAsync.ThrowIfNull(nameof(appraisersManagerAsync));
            OutputManagerAsync = outputManagerAsync.ThrowIfNull(nameof(outputManagerAsync));

            _boundedCapacity = boundedCapacity;
            _dataFlowOptions = new DataflowBlockOptions { BoundedCapacity = _boundedCapacity };
        }

        private async Task<EStatus> GetThingsNames(BufferBlock<String> queue, String storageName)
        {
            if (String.IsNullOrEmpty(storageName)) return EStatus.Error;

            try
            {
                Boolean status = await InputManagerAsync.GetNames(queue, storageName);
                if (status)
                {
                    SGlobalMessageHandler.OutputMessage("Things were successfully gotten.");
                    return EStatus.Ok;
                }

                SGlobalMessageHandler.OutputMessage($"No Things were found in \"{storageName}\".");
                return EStatus.Nothing;
            }
            catch (Exception ex)
            {
                s_logger.Error(ex, "Exception occured during input work.");
                return EStatus.InputError;
            }
        }

        private async Task<EStatus> RequestData(BufferBlock<String> entitiesQueue,
            Dictionary<Type, BufferBlock<Data.CBasicInfo>> responsesQueues)
        {
            try
            {
                Boolean status = await CrawlersManagerAsync.CollectAllResponses(
                    entitiesQueue, responsesQueues, _dataFlowOptions
                );
                if (status)
                {
                    SGlobalMessageHandler.OutputMessage(
                        "Crawlers have received responses from services."
                    );
                    return EStatus.Ok;
                }

                SGlobalMessageHandler.OutputMessage(
                    "Crawlers have not received responses from services. Result is empty."
                );
                return EStatus.Nothing;
            }
            catch (Exception ex)
            {
                s_logger.Error(ex, "Exception occured during collecting data.");
                return EStatus.RequestError;
            }
        }

        private async Task<EStatus> AppraiseThings(
            Dictionary<Type, BufferBlock<Data.CBasicInfo>> entitiesInfoQueues,
            Dictionary<Type, BufferBlock<Data.CRatingDataContainer>> entitiesRatingQueues)
        {
            try
            {
                Boolean status = await AppraisersManagerAsync.GetAllRatings(
                    entitiesInfoQueues, entitiesRatingQueues, _dataFlowOptions
                );
                if (status)
                {
                    SGlobalMessageHandler.OutputMessage(
                        "Appraisers have calculated ratings successfully."
                    );
                    return EStatus.Ok;
                }

                SGlobalMessageHandler.OutputMessage(
                    "Appraisers have not calculated ratings. Result is empty."
                );
                return EStatus.Nothing;
            }
            catch (Exception ex)
            {
                s_logger.Error(ex, "Exception occured during appraising work.");
                return EStatus.AppraiseError;
            }
        }

        private async Task<EStatus> SaveResults(
            Dictionary<Type, BufferBlock<Data.CRatingDataContainer>> ratings)
        {
            try
            {
                Boolean status = await OutputManagerAsync.SaveResults(ratings, String.Empty);
                if (status)
                {
                    SGlobalMessageHandler.OutputMessage("Ratings was saved successfully.");
                    return EStatus.Ok;
                }

                SGlobalMessageHandler.OutputMessage("Ratings wasn't saved.");
                return EStatus.OutputUnsaved;
            }
            catch (Exception ex)
            {
                s_logger.Error(ex, "Exception occured during output work.");
                return EStatus.OutputError;
            }
        }

        public async Task<EStatus> Run(String storageName)
        {
            SGlobalMessageHandler.OutputMessage("Shell started work.");
            s_logger.Info("Shell started work.");

            var inputQueue = new BufferBlock<String>(_dataFlowOptions);

            var responsesQueues = new Dictionary<Type, BufferBlock<Data.CBasicInfo>>();

            var ratingsQueues = new Dictionary<Type, BufferBlock<Data.CRatingDataContainer>>();

            // Input component work.
            Task<EStatus> inputStatus = GetThingsNames(inputQueue, storageName);

            // Crawlers component work.
            Task<EStatus> crawlersStatus = RequestData(inputQueue, responsesQueues);

            // Appraisers component work.
            Task<EStatus> appraisersStatus = AppraiseThings(responsesQueues, ratingsQueues);

            // Output component work.
            Task<EStatus> outputStatus = SaveResults(ratingsQueues);

            Task<EStatus[]> statusesTask = Task.WhenAll(inputStatus, crawlersStatus,
                                                        appraisersStatus, outputStatus);

            Task responsesQueuesTasks = Task.WhenAll(
                responsesQueues.Values.Select(bufferBlock => bufferBlock.Completion)
            );
            Task ratingsQueuesTasks = Task.WhenAll(
                ratingsQueues.Values.Select(bufferBlock => bufferBlock.Completion)
            );

            await Task.WhenAll(statusesTask, inputQueue.Completion, responsesQueuesTasks,
                               ratingsQueuesTasks);

            // FIX ME: if there are error statuses need to create aggregate status which contains
            // more details then simple EStatus.Error value.
            EStatus[] statuses = await statusesTask;
            if (statuses.Any(status => status != EStatus.Ok))
            {
                SGlobalMessageHandler.OutputMessage(
                    "Shell got error status during data processing."
                );
                s_logger.Info("Shell got error status during data processing.");
                return EStatus.Error;
            }

            s_logger.Info("Shell finished work successfully.");
            SGlobalMessageHandler.OutputMessage("Shell finished work successfully.");
            return EStatus.Ok;
        }
    }
}
