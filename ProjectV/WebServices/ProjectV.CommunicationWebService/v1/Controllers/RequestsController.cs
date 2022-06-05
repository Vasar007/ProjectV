using System.Threading.Tasks;
using Acolyte.Assertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectV.CommunicationWebService.v1.Domain.Configuration;
using ProjectV.CommunicationWebService.v1.Domain.Processing;
using ProjectV.Logging;
using ProjectV.Models.WebServices.Requests;

namespace ProjectV.CommunicationWebService.v1.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public sealed class RequestsController : ControllerBase
    {
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor<RequestsController>();

        private readonly IConfigurationReceiver _configurationReceiver;
        private readonly IProcessingResponseReceiver _processingResponseReceiver;


        public RequestsController(
            IConfigurationReceiver configurationReceiver,
            IProcessingResponseReceiver processingResponseReceiver)
        {
            _configurationReceiver = configurationReceiver.ThrowIfNull(nameof(configurationReceiver));
            _processingResponseReceiver = processingResponseReceiver.ThrowIfNull(nameof(processingResponseReceiver));
        }

        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<string> GetInfo()
        {
            return Ok("Process your data by ProjectV service with POST request.");
        }

        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ProcessJobRequest(
            StartJobParamsRequest jobParams)
        {
            _logger.Info("Got request to add in processing queue.");

            var jobDataResult = await _configurationReceiver.ReceiveConfigForRequestAsync(jobParams);
            if (!jobDataResult.IsSuccess)
            {
                return BadRequest(jobDataResult.Error);
            }

            // Process the things with received config.
            // "_configurationReceiver" ensures that result will return not-null config.
            var responseResult = await _processingResponseReceiver.ReceiveProcessingResponseAsync(jobDataResult.Ok!);
            if (!responseResult.IsSuccess)
            {
                return BadRequest(responseResult.Error);
            }

            return Ok(responseResult.Ok);
        }
    }
}
