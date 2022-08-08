using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MikeyT.EnvironmentSettingsNS.Interface;
using OmegaInterop.Core;
using OmegaInterop.Weather;

namespace OmegaService.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SomeWebEndpointController : ControllerBase
    {
        private readonly ILogger<SomeWebEndpointController> _logger;
        private readonly IWeatherClient _weatherClient;
        private readonly ICoreClient _coreClient;

        public SomeWebEndpointController(
            ILogger<SomeWebEndpointController> logger,
            IEnvironmentSettings envSettings,
            IWeatherClient weatherClient,
            ICoreClient coreClient)
        {
            _logger = logger;
            _weatherClient = weatherClient;
            _coreClient = coreClient;
        }

        [HttpGet]
        public async Task<IActionResult> GetStuff()
        {
            _logger.LogInformation("SomeWebEndpointController default route called - will call into core service");
            _logger.LogInformation("CORE_URL_BASE: {CoreUrlBase}", _coreClient.GetHttpClient().BaseAddress);
            _logger.LogInformation("WEATHER_URL_BASE: {WeatherUrlBase}", _weatherClient.GetHttpClient().BaseAddress);

            var coreDummyMessage = await _coreClient.GetCoreDummyMessage();
            var forecasts = await _weatherClient.GetRandomWeatherForecasts();

            return new JsonResult(new
            {
                fromSomeWeb = "stuff from web controller",
                fromCore = coreDummyMessage,
                fromWeather = forecasts
            });
        }

        [HttpGet("OmegaUsers")]
        public async Task<ActionResult<IEnumerable<OmegaUser>>> GetOmegaUsers()
        {
            var omegaUsers = await _coreClient.GetOmegaUsers();
            return Ok(omegaUsers);
        }
    }
}
