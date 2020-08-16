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
    /// <summary>
    /// Main class of service that links the rest of the classes into a single entity.
    /// </summary>
    public sealed class Shell : IDisposable
    {
        /// <summary>
        /// Logger instance for current class.
        /// </summary>
        private static readonly ILogger _logger = LoggerFactory.CreateLoggerFor<Shell>();

        private readonly int _boundedCapacity;

        /// <summary>
        /// Manager to interact with input data.
        /// </summary>
        public IO.Input.InputManager InputManager { get; }

        /// <summary>
        /// Manager to collect data about The Things from different sources.
        /// </summary>
        public Crawlers.CrawlersManager CrawlersManager { get; }

        /// <summary>
        /// Manager to appraise of information collected.
        /// </summary>
        public Appraisers.AppraisersManager AppraisersManager { get; }

        /// <summary>
        /// Manager to save service results.
        /// </summary>
        public IO.Output.OutputManager OutputManager { get; }


        /// <summary>
        /// Default constructor which initialize all managers.
        /// </summary>
        /// <param name="inputManager">Initialized input manager.</param>
        /// <param name="crawlersManager">Initialized crawlers manager.</param>
        /// <param name="appraisersManager">Initialized appraisers manager.</param>
        /// <param name="outputManager">Initialized output manager.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="inputManager" /> or <paramref name="crawlersManager" /> or
        /// <paramref name="appraisersManager" /> or <paramref name="outputManager" /> is 
        /// <c>null</c>.
        /// </exception>
        public Shell(
            IO.Input.InputManager inputManager,
            Crawlers.CrawlersManager crawlersManager,
            Appraisers.AppraisersManager appraisersManager,
            IO.Output.OutputManager outputManager,
            int boundedCapacity)
        {
            InputManager = inputManager.ThrowIfNull(nameof(inputManager));
            CrawlersManager = crawlersManager.ThrowIfNull(nameof(crawlersManager));
            AppraisersManager = appraisersManager.ThrowIfNull(nameof(appraisersManager));
            OutputManager = outputManager.ThrowIfNull(nameof(outputManager));

            _boundedCapacity = boundedCapacity; // Not using this parameter now.
        }

        public static ShellBuilderDirector CreateBuilderDirector(XDocument configuration)
        {
            return new ShellBuilderDirector(new ShellBuilderFromXDocument(configuration));
        }

        /// <summary>
        /// Launches the whole cycle of collecting and processing data.
        /// </summary>
        /// <param name="storageName">Name of the input source.</param>
        /// <returns>Status value depending on the result of the service.</returns>
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

        /// <summary>
        /// Releases all acquired resources.
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;

            CrawlersManager.Dispose();

            _disposed = true;
        }

        #endregion

        /// <summary>
        /// Saves ratings to the output sources.
        /// </summary>
        /// <returns><c>true</c> if the save was successful, <c>false</c> otherwise.</returns>
        private async Task<ServiceStatus> SaveResults(OutputtersFlow outputtersFlow)
        {
            try
            {
                bool status = await OutputManager.SaveResults(
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
            InputtersFlow inputtersFlow = InputManager.CreateFlow(storageName);

            // Crawlers component work.
            CrawlersFlow crawlersFlow = CrawlersManager.CreateFlow();

            // Appraisers component work.
            AppraisersFlow appraisersFlow = AppraisersManager.CreateFlow();

            // Output component work.
            OutputtersFlow outputtersFlow =
                OutputManager.CreateFlow(storageName: string.Empty);

            // Constructing pipeline.
            inputtersFlow.LinkTo(crawlersFlow);
            crawlersFlow.LinkTo(appraisersFlow);
            appraisersFlow.LinkTo(outputtersFlow);

            return new DataflowPipeline(inputtersFlow, outputtersFlow);
        }
    }
}
