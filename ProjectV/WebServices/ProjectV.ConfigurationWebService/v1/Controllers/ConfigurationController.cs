using System;
using Acolyte.Assertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectV.ConfigurationWebService.v1.Domain;
using ProjectV.Logging;
using ProjectV.Models.Configuration;
using ProjectV.Models.WebService;

namespace ProjectV.ConfigurationWebService.v1.Controllers
{
    [Route("api/v{version:apiVersion}/configuration")]
    [ApiController]
    public sealed class ConfigurationController : ControllerBase
    {
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor<ConfigurationController>();

        private readonly IConfigCreator _configCreator;


        public ConfigurationController(IConfigCreator configCreator)
        {
            _configCreator = configCreator.ThrowIfNull(nameof(configCreator));
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<string> GetInfo()
        {
            return Ok("Get proper configuration with POST request.");
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<ConfigurationXml> PostConfiguration(
            ConfigRequirements configRequirements)
        {
            try
            {
                ConfigurationXml configuration = _configCreator.CreateConfigBasedOnRequirements(
                    configRequirements
                );
                return Ok(configuration);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception occurred during configuration creating.");
                return BadRequest(configRequirements);
            }
        }
    }
}
