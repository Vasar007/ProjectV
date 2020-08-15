using System;
using System.Threading.Tasks;
using Acolyte.Assertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectV.DataAccessLayer.EntityFramework;
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

        private readonly IJobInfoService _jobInfoService;


        public ProcessingController(
            ITargetServiceCreator serviceCreator,
            IJobInfoService jobInfoService)
        {
            _serviceCreator = serviceCreator.ThrowIfNull(nameof(serviceCreator));
            _jobInfoService = jobInfoService.ThrowIfNull(nameof(jobInfoService));
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<string> GetInfo()
        {
            return Ok("Create procesiing task by POST request");
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProcessingResponse>> PostRequestData(
            RequestData requestData)
        {
            _logger.Info("Processing data request.");

            try
            {
                IServiceRequestProcessor requestProcessor = _serviceCreator.CreateRequestProcessor(
                    requestData.ConfigurationXml.ServiceType, _jobInfoService
                );

                ProcessingResponse response = await requestProcessor.ProcessRequest(requestData);
                if (response.Metadata.ResultStatus != ServiceStatus.Ok)
                {
                    return BadRequest(response);
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception occurred during data processing.");
                return BadRequest(requestData);
            }
        }
    }
}
