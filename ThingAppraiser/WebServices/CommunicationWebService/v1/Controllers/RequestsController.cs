using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ThingAppraiser.Data.Models;
using ThingAppraiser.Logging;
using ThingAppraiser.CommunicationWebService.v1.Domain;

namespace ThingAppraiser.CommunicationWebService.v1.Controllers
{
    [Route("api/v{version:apiVersion}/requests")]
    [ApiController]
    public class RequestsController : ControllerBase
    {
        private static readonly LoggerAbstraction _logger =
            LoggerAbstraction.CreateLoggerInstanceFor<RequestsController>();

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
        public ActionResult<string> GetInfo()
        {
            return "You can get request processing your data by ThingsAppraiser service.";
        }

        [HttpPost]
        public async Task<ActionResult<ProcessingResponse>> PostInitialRequest(
            RequestParams requestParams)
        {
            try
            {
                RequestData requestData =
                    await _configurationReceiver.ReceiveConfigForRequestAsync(requestParams);

                ProcessingResponse response =
                    await _processingResponseReceiver.ReceiveProcessingResponseAsync(requestData);

                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception occurred during request handling.");
            }
            return BadRequest(requestParams);
        }
    }
}
