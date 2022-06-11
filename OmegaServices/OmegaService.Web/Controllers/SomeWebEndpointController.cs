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
        private readonly string CORE_URL_BASE;
        private readonly string WEATHER_URL_BASE;
        private static Random _random = new Random();

        private static readonly HttpClient _httpClient = new HttpClient(new HttpClientHandler
        {
            ClientCertificateOptions = ClientCertificateOption.Manual,
            ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => true
        });

        public SomeWebEndpointController(ILogger<SomeWebEndpointController> logger, IEnvSettings envSettings)
        {
            _logger = logger;

            CORE_URL_BASE = $"http://{envSettings.GetString(WebEnvSettings.CORE_HOST)}:{envSettings.GetString(WebEnvSettings.CORE_PORT)}/api/";
            WEATHER_URL_BASE = $"http://{envSettings.GetString(WebEnvSettings.WEATHER_HOST)}:{envSettings.GetString(WebEnvSettings.WEATHER_PORT)}/api/";
        }

        [HttpGet]
        public async Task<IActionResult> GetStuff()
        {
            _logger.LogInformation("SomeWebEndpointController default route called - will call into core service");
            _logger.LogInformation($"CORE_URL_BASE: {CORE_URL_BASE}");
            _logger.LogInformation($"WEATHER_URL_BASE: {WEATHER_URL_BASE}");

            string coreResponseString = await _httpClient.GetStringAsync($"{CORE_URL_BASE}Core/DummyCore");

            string weatherResponseString = await _httpClient.GetStringAsync($"{WEATHER_URL_BASE}Weather/FakeWeather");

            return new JsonResult(new
            {
                fromSomeWeb = "stuff from web controller",
                fromCore = coreResponseString,
                fromWeather = weatherResponseString
            });
        }
    }
}
