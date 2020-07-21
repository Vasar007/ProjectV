using System;
using System.Threading.Tasks;
using Acolyte.Assertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using ProjectV.Logging;
using ProjectV.TelegramBotWebService.v1.Domain;

namespace ProjectV.TelegramBotWebService.v1.Controllers
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<string> GetInfo()
        {
            return Ok("Interaction API for TelegramBot");
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Post([FromBody] Update update)
        {
            try
            {
                await _updateService.ProcessUpdateMessage(update);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception occurred during bot service work.");
                return BadRequest(update);
            }
        }
    }
}
