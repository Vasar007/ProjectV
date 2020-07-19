using System;
using System.Threading.Tasks;
using Acolyte.Assertions;
using Microsoft.AspNetCore.Mvc;
using ProjectV.DAL.EntityFramework;
using ProjectV.Logging;
using ProjectV.Models.Internal;
using ProjectV.Models.WebService;
using ProjectV.ProcessingWebService.v1.Domain;

namespace ProjectV.ProcessingWebService.v1.Controllers
{
    [Route("api/v{version:apiVersion}/processing")]
    [ApiController]
    public sealed class ProcessingController : ControllerBase
    {
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor<ProcessingController>();

        private readonly ITargetServiceCreator _serviceCreator;

        private readonly ITaskInfoService _taskRepository;


        public ProcessingController(
            ITargetServiceCreator serviceCreator,
            ITaskInfoService taskRepository)
        {
            _serviceCreator = serviceCreator.ThrowIfNull(nameof(serviceCreator));
            _taskRepository = taskRepository.ThrowIfNull(nameof(taskRepository));
        }

        [HttpGet]
        public ActionResult<string> GetInfo()
        {
            return "Create procesiing task by POST request";
        }

        [HttpPost]
        public async Task<ActionResult<ProcessingResponse>> PostRequestData(
            RequestData requestData)
        {
            _logger.Info("Processing data request.");

            try
            {
                IServiceRequestProcessor requestProcessor = _serviceCreator.CreateRequestProcessor(
                    requestData.ConfigurationXml.ServiceType, _taskRepository
                );

                ProcessingResponse response = await requestProcessor.ProcessRequest(requestData);
                if (response.Metadata.ResultStatus != ServiceStatus.Ok)
                {
                    return BadRequest(response);
                }
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception occurred during data processing.");
            }
            return BadRequest(requestData);
        }
    }
}
