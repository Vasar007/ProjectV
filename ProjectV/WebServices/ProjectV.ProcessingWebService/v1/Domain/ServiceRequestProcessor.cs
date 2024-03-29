﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Acolyte.Assertions;
using ProjectV.Configuration;
using ProjectV.DataAccessLayer.Services.Jobs;
using ProjectV.Executors;
using ProjectV.IO.Input.WebService;
using ProjectV.IO.Output.WebService;
using ProjectV.Logging;
using ProjectV.Models.Internal;
using ProjectV.Models.Internal.Jobs;
using ProjectV.Models.WebServices.Responses;
using ProjectV.TmdbService;

namespace ProjectV.ProcessingWebService.v1.Domain
{
    public sealed class ServiceRequestProcessor : IServiceRequestProcessor
    {
        /// <summary>
        /// Logger instance for current class.
        /// </summary>
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor(typeof(ServiceRequestProcessor));

        private readonly IJobInfoService _jobInfoService;


        public ServiceRequestProcessor(
            IJobInfoService jobInfoService)
        {
            _jobInfoService = jobInfoService.ThrowIfNull(nameof(jobInfoService));
        }

        #region IServiceRequestProcessor Implementation

        public async Task<ProcessingResponse> ProcessRequest(StartJobDataResponce jobData)
        {
            _logger.Info("Processing request with async processor.");

            var inputTransmitter = new InputTransmitter(jobData.ThingNames);
            var outputTransmitter = new OutputTransmitter();

            SimpleExecutor simpleExecutor = await CreateExecutorAsync(jobData);

            IReadOnlyList<ServiceStatus> resultStatuses = await simpleExecutor.ExecuteAsync(
                jobData, inputTransmitter, outputTransmitter
            );

            ServiceStatus status = resultStatuses.Single();

            await LogResult(simpleExecutor);

            IReadOnlyList<IReadOnlyList<RatingDataContainer>> results =
                outputTransmitter.GetResults();

            var response = new ProcessingResponse
            {
                Metadata = new ProcessingResponseMetadata
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

        private async Task<SimpleExecutor> CreateExecutorAsync(StartJobDataResponce requestData)
        {
            // TODO: refactor this code.
            var jobInfo = JobInfo.Create(
                name: "Simple Async Job",
                config: XmlConfigCreator.TransformConfigToXDocument(
                            requestData.ConfigurationXml
                        ).ToString()
            );

            await _jobInfoService.AddAsync(jobInfo);

            return new SimpleExecutor(
                jobInfo: jobInfo,
                executionsNumber: 1,
                delayTime: TimeSpan.Zero
            );
        }

        private async Task LogResult(IExecutor executor)
        {
            JobInfo? jobInfo = await _jobInfoService.FindByIdAsync(executor.Id);
            if (jobInfo is null)
            {
                _logger.Info($"No job info found for executor '{executor.Id.ToString()}'.");
                return;
            }

            _logger.Info($"Final job info: {jobInfo.ToLogString()}");
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
