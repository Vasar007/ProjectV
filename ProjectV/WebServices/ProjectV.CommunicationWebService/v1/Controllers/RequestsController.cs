using System.Threading.Tasks;
using Acolyte.Assertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectV.CommunicationWebService.v1.Domain.Configuration;
using ProjectV.CommunicationWebService.v1.Domain.Processing;
using ProjectV.Logging;
using ProjectV.Models.WebServices.Requests;
using ProjectV.Models.WebServices.Responses;

namespace ProjectV.CommunicationWebService.v1.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public sealed class RequestsController : ControllerBase
    {
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor<RequestsController>();

        private readonly IConfigurationReceiverAsync _configurationReceiver;

        private readonly IProcessingResponseReceiverAsync _processingResponseReceiver;

        public RequestsController(
            IConfigurationReceiverAsync configurationReceiver,
            IProcessingResponseReceiverAsync processingResponseReceiver)
        {
            _configurationReceiver = configurationReceiver.ThrowIfNull(
                nameof(configurationReceiver)
            );

            _processingResponseReceiver = processingResponseReceiver.ThrowIfNull(
                nameof(processingResponseReceiver)
            );
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
        //[Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ProcessingResponse>> ProcessJobRequest(
            StartJobParamsRequest jobParams)
        {
            Foo();

            _logger.Info("Got request to add in processing queue.");

            StartJobDataResponce jobData =
                await _configurationReceiver.ReceiveConfigForRequestAsync(jobParams);

            ProcessingResponse response =
                await _processingResponseReceiver.ReceiveProcessingResponseAsync(jobData);

            return Ok(response);
        }

        private void Foo()
        {
            throw new System.Exception("Try again.");
        }
    }
}
