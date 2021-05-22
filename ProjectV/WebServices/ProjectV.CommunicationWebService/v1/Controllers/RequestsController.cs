using System;
using System.Threading.Tasks;
using Acolyte.Assertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectV.CommunicationWebService.v1.Domain;
using ProjectV.Logging;
using ProjectV.Models.WebService;

namespace ProjectV.CommunicationWebService.v1.Controllers
{
    [Route("api/v{version:apiVersion}/requests")]
    [ApiController]
    public sealed class RequestsController : ControllerBase
    {
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor<RequestsController>();

        private readonly IConfigurationReceiverAsync _configurationReceiver;

        private readonly IProcessingResponseReceiverAsync _processingResponseReceiver;

        public RequestsController(IConfigurationReceiverAsync configurationReceiver,
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<string> GetInfo()
        {
            return Ok("You can get request processing your data by ThingsAppraiser service.");
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProcessingResponse>> PostInitialRequest(
            RequestParams requestParams)
        {
            try
            {
                RequestData requestData =
                    await _configurationReceiver.ReceiveConfigForRequestAsync(requestParams);

                ProcessingResponse response =
                    await _processingResponseReceiver.ReceiveProcessingResponseAsync(requestData);

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception occurred during request handling.");
                return BadRequest(requestParams);
            }
        }
    }
}
