using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ThingAppraiser.Extensions;
using ThingAppraiser.Logging;
using ThingAppraiser.Models.Internal;
using ThingAppraiser.Models.WebService;
using ThingAppraiser.ProcessingWebService.v1.Domain;

namespace ThingAppraiser.ProcessingWebService.v1.Controllers
{
    [Route("api/v{version:apiVersion}/processing")]
    [ApiController]
    public sealed class ProcessingController : ControllerBase
    {
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor<ProcessingController>();

        private readonly ITargetServiceCreator _serviceCreator;


        public ProcessingController(ITargetServiceCreator serviceCreator)
        {
            _serviceCreator = serviceCreator.ThrowIfNull(nameof(serviceCreator));
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
                    requestData.ConfigurationXml.ServiceType
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
