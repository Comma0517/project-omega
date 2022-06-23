using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using EnvironmentSettings.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OmegaModel.Core;
using OmegaModel.Weather;

namespace OmegaService.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SomeWebEndpointController : ControllerBase
    {
        private readonly ILogger<SomeWebEndpointController> _logger;
        private readonly string _coreUrlBase;
        private readonly string _weatherUrlBase;

        private static readonly HttpClient _httpClient = new HttpClient(new HttpClientHandler
        {
            ClientCertificateOptions = ClientCertificateOption.Manual,
            ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => true
        });

        public SomeWebEndpointController(ILogger<SomeWebEndpointController> logger, IEnvSettings envSettings)
        {
            _logger = logger;

            _coreUrlBase = $"http://{envSettings.GetString(WebEnvSettings.CORE_HOST)}:{envSettings.GetString(WebEnvSettings.CORE_PORT)}/api/Core";
            _weatherUrlBase = $"http://{envSettings.GetString(WebEnvSettings.WEATHER_HOST)}:{envSettings.GetString(WebEnvSettings.WEATHER_PORT)}/api/Weather";
        }

        [HttpGet]
        public async Task<IActionResult> GetStuff()
        {
            _logger.LogInformation("SomeWebEndpointController default route called - will call into core service");
            _logger.LogInformation($"CORE_URL_BASE: {_coreUrlBase}");
            _logger.LogInformation($"WEATHER_URL_BASE: {_weatherUrlBase}");

            var coreResponseString = await _httpClient.GetStringAsync($"{_coreUrlBase}/DummyCore");
            var forecasts = await _httpClient.GetFromJsonAsync($"{_weatherUrlBase}/WeatherForecast", typeof(List<WeatherForecast>));

            return new JsonResult(new
            {
                fromSomeWeb = "stuff from web controller",
                fromCore = coreResponseString,
                fromWeather = forecasts
            });
        }
        
        [HttpGet("OmegaUser/All")]
        public async Task<ActionResult<IEnumerable<OmegaUser>>> GetOmegaUsers()
        {
            var omegaUsers = await  _httpClient.GetFromJsonAsync($"{_coreUrlBase}/OmegaUser/All", typeof(List<OmegaUser>));
            return Ok(omegaUsers);
        }
    }
}
