using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using ThingAppraiser.Extensions;
using ThingAppraiser.Logging;
using ThingAppraiser.TelegramBotWebService.v1.Domain;

namespace ThingAppraiser.TelegramBotWebService.v1.Controllers
{
    [Route("api/v{version:apiVersion}/update")]
    [ApiController]
    public sealed class UpdateController : Controller
    {
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor<UpdateController>();

        private readonly IUpdateServiceAsync _updateService;


        public UpdateController(IUpdateServiceAsync updateService)
        {
            _updateService = updateService.ThrowIfNull(nameof(updateService));
        }

        [HttpGet]
        public ActionResult<string> GetInfo()
        {
            return "Interaction API for TelegramBot";
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Update update)
        {
            try
            {
                await _updateService.ProcessUpdateMessage(update);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception occurred during bot service work.");
            }
            return BadRequest(update);
        }
    }
}
