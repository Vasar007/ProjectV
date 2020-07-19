using System;
using System.Threading.Tasks;
using System.Xml.Linq;
using Acolyte.Assertions;
using ProjectV.Communication;
using ProjectV.Core.ShellBuilders;
using ProjectV.DataPipeline;
using ProjectV.Logging;
using ProjectV.Models.Internal;

namespace ProjectV.Core
{
    public sealed class ShellAsync : IDisposable
    {
        private static readonly ILogger _logger = LoggerFactory.CreateLoggerFor<ShellAsync>();

        private readonly int _boundedCapacity;

        public IO.Input.InputManagerAsync InputManagerAsync { get; }

        public Crawlers.CrawlersManagerAsync CrawlersManagerAsync { get; }

        public Appraisers.AppraisersManagerAsync AppraisersManagerAsync { get; }

        public IO.Output.OutputManagerAsync OutputManagerAsync { get; }


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

            _boundedCapacity = boundedCapacity; // Not using this parameter now.
        }

        public static ShellAsyncBuilderDirector CreateBuilderDirector(XDocument configuration)
        {
            return new ShellAsyncBuilderDirector(new ShellAsyncBuilderFromXDocument(configuration));
        }

        public async Task<ServiceStatus> Run(string storageName)
        {
            const string startMessage = "Shell started work.";
            GlobalMessageHandler.OutputMessage(startMessage);
            _logger.Info(startMessage);

            DataflowPipeline dataflowPipeline;
            try
            {
                dataflowPipeline = ConstructPipeline(storageName);

                // Start processing data.
                // And wait for the last block in the pipeline to process all messages.
                await dataflowPipeline.Execute(storageName);
            }
            catch (Exception ex)
            {
                const string failureMessage = "Shell got an exception during data processing.";
                _logger.Error(ex, failureMessage);
                GlobalMessageHandler.OutputMessage(failureMessage);

                return ServiceStatus.Error;
            }

            ServiceStatus status = await SaveResults(dataflowPipeline.OutputtersFlow);
            if (status != ServiceStatus.Ok)
            {
                string failureMessage = $"Shell got \"{status.ToString()}\" status " +
                                        "during data saving.";
                GlobalMessageHandler.OutputMessage(failureMessage);
                _logger.Info(failureMessage);

                return status;
            }

            const string successfulMessage = "Shell finished work successfully.";
            _logger.Info(successfulMessage);
            GlobalMessageHandler.OutputMessage(successfulMessage);

            return ServiceStatus.Ok;
        }

        #region IDisposable Implementation

        /// <summary>
        /// Boolean flag used to show that object has already been disposed.
        /// </summary>
        private bool _disposed;

        public void Dispose()
        {
            if (_disposed) return;

            CrawlersManagerAsync.Dispose();

            _disposed = true;
        }

        #endregion

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

                GlobalMessageHandler.OutputMessage("Ratings was not saved.");
                return ServiceStatus.OutputUnsaved;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception occured during saving results work.");
                return ServiceStatus.OutputError;
            }
        }

        private DataflowPipeline ConstructPipeline(string storageName)
        {
            // Input component work.
            InputtersFlow inputtersFlow = InputManagerAsync.CreateFlow(storageName);

            // Crawlers component work.
            CrawlersFlow crawlersFlow = CrawlersManagerAsync.CreateFlow();

            // Appraisers component work.
            AppraisersFlow appraisersFlow = AppraisersManagerAsync.CreateFlow();

            // Output component work.
            OutputtersFlow outputtersFlow =
                OutputManagerAsync.CreateFlow(storageName: string.Empty);

            // Constructing pipeline.
            inputtersFlow.LinkTo(crawlersFlow);
            crawlersFlow.LinkTo(appraisersFlow);
            appraisersFlow.LinkTo(outputtersFlow);

            return new DataflowPipeline(inputtersFlow, outputtersFlow);
        }
    }
}
