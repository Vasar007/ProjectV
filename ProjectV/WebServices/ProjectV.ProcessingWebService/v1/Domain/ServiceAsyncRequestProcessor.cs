using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Acolyte.Assertions;
using ProjectV.Configuration;
using ProjectV.DAL.EntityFramework;
using ProjectV.IO.Input.WebService;
using ProjectV.IO.Output.WebService;
using ProjectV.Models.Internal;
using ProjectV.Models.Internal.Tasks;
using ProjectV.Models.WebService;
using ProjectV.TaskService;
using ProjectV.TmdbService;
using ProjectV.Logging;

namespace ProjectV.ProcessingWebService.v1.Domain
{
    public sealed class ServiceAsyncRequestProcessor : IServiceRequestProcessor
    {
        /// <summary>
        /// Logger instance for current class.
        /// </summary>
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor(typeof(ServiceAsyncRequestProcessor));

        private readonly ITaskInfoService _taskInfoService;


        public ServiceAsyncRequestProcessor(
            ITaskInfoService taskInfoService)
        {
            _taskInfoService = taskInfoService.ThrowIfNull(nameof(taskInfoService));
        }

        #region IServiceRequestProcessor Implementation

        public async Task<ProcessingResponse> ProcessRequest(RequestData requestData)
        {
            _logger.Info("Processing request with async processor.");

            var inputTransmitter = new InputTransmitterAsync(requestData.ThingNames);
            var outputTransmitter = new OutputTransmitterAsync();

            SimpleTask simpleTask = await CreateTaskAsync(requestData);

            IReadOnlyList<ServiceStatus> resultStatuses = await simpleTask.ExecuteAsync(
                requestData, inputTransmitter, outputTransmitter
            );

            ServiceStatus status = resultStatuses.Single();

            await LogResult(simpleTask);

            IReadOnlyList<IReadOnlyList<RatingDataContainer>> results =
                outputTransmitter.GetResults();

            var response = new ProcessingResponse
            {
                Metadata = new ResponseMetadata
                {
                    CommonResultsNumber = results.Sum(rating => rating.Count),
                    CommonResultCollectionsNumber = results.Count,
                    ResultStatus = status,
                    OptionalData = CreateOptionalData()
                },
                RatingDataContainers = results
            };

            _logger.Info("Request was successfully processed by async processor.");
            return response;
        }

        #endregion

        private async Task<SimpleTask> CreateTaskAsync(RequestData requestData)
        {
            // TODO: refactor this code.
            var taskInfo = TaskInfo.Create(
                name: "Simple Async Task",
                config: XmlConfigCreator.TransformConfigToXDocument(
                            requestData.ConfigurationXml).ToString()
            );

            await _taskInfoService.AddAsync(taskInfo);

            return new SimpleTask(
                taskInfo: taskInfo,
                executionsNumber: 1,
                delayTime: TimeSpan.Zero
            );
        }

        private async Task LogResult(IExecutableTask executableTask)
        {
            TaskInfo taskInfo = await _taskInfoService.GetByIdAsync(executableTask.Id);

            _logger.Info($"Final task info: {taskInfo.ToLogString()}");
        }

        private IReadOnlyDictionary<string, IOptionalData> CreateOptionalData()
        {
            var result = new Dictionary<string, IOptionalData>();
            if (TmdbServiceConfiguration.HasValue)
            {
                result.Add(
                    nameof(TmdbServiceConfiguration),
                    TmdbServiceConfiguration.Configuration
                );
            }

            return result;
        }
    }
}
