using System.Threading.Tasks;
using Acolyte.Assertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectV.TelegramBotWebService.v1.Domain;
using Telegram.Bot.Types;

namespace ProjectV.TelegramBotWebService.v1.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public sealed class UpdateController : Controller
    {
        private readonly IUpdateService _updateService;


        public UpdateController(IUpdateService updateService)
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
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Post([FromBody] Update update)
        {
            await _updateService.ProcessUpdateMessage(update);
            return Ok();
        }
    }
}
