using System;
using System.Net.Http;
using System.Threading.Tasks;
using EnvironmentSettings.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace OmegaService.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SomeWebEndpointController : ControllerBase
    {
        private readonly ILogger<SomeWebEndpointController> _logger;
        private readonly string _coreUrlBase;
        private readonly string _weatherUrlBase;
        private static Random _random = new Random();

        private static readonly HttpClient HttpClient = new HttpClient(new HttpClientHandler
        {
            ClientCertificateOptions = ClientCertificateOption.Manual,
            ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => true
        });

        public SomeWebEndpointController(ILogger<SomeWebEndpointController> logger, IEnvSettings envSettings)
        {
            _logger = logger;

            _coreUrlBase = $"http://{envSettings.GetString(WebEnvSettings.CORE_HOST)}:{envSettings.GetString(WebEnvSettings.CORE_PORT)}/api/";
            _weatherUrlBase = $"http://{envSettings.GetString(WebEnvSettings.WEATHER_HOST)}:{envSettings.GetString(WebEnvSettings.WEATHER_PORT)}/api/";
        }

        [HttpGet]
        public async Task<IActionResult> GetStuff()
        {
            _logger.LogInformation("SomeWebEndpointController default route called - will call into core service");
            _logger.LogInformation($"CORE_URL_BASE: {_coreUrlBase}");
            _logger.LogInformation($"WEATHER_URL_BASE: {_weatherUrlBase}");

            var coreResponseString = await HttpClient.GetStringAsync($"{_coreUrlBase}Core/DummyCore");

            var weatherResponseString = await HttpClient.GetStringAsync($"{_weatherUrlBase}Weather/FakeWeather");

            return new JsonResult(new
            {
                fromSomeWeb = "stuff from web controller",
                fromCore = coreResponseString,
                fromWeather = weatherResponseString
            });
        }
    }
}
