using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace Omega.Controllers.Web
{
    [ApiController]
    [Route("api/[controller]")]
    public class SomeWebEndpointController : ControllerBase
    {
        private readonly ILogger<SomeWebEndpointController> _logger;
        private readonly string CORE_URL_BASE;
        private readonly string WEATHER_URL_BASE;
        private static Random random = new Random();
        private static HttpClient httpClient = new HttpClient(new HttpClientHandler
        {
            ClientCertificateOptions = ClientCertificateOption.Manual,
            ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => true
        });

        public SomeWebEndpointController(ILogger<SomeWebEndpointController> logger)
        {
            _logger = logger;
            CORE_URL_BASE = $"http://{EnvHelper.CORE_HOST}:{EnvHelper.CORE_PORT}/api/";
            WEATHER_URL_BASE = $"http://{EnvHelper.WEATHER_HOST}:{EnvHelper.WEATHER_PORT}/api/";
        }

        [HttpGet]
        public async Task<IActionResult> GetStuff()
        {
            _logger.LogInformation("SomeWebController default route called - will call into core service");
            _logger.LogInformation($"CORE_URL_BASE: {CORE_URL_BASE}");
            _logger.LogInformation($"WEATHER_URL_BASE: {WEATHER_URL_BASE}");
            
            string coreResponseString = await httpClient.GetStringAsync($"{CORE_URL_BASE}Core/DummyCore");
            
            string weatherResponseString = await httpClient.GetStringAsync($"{WEATHER_URL_BASE}Weather/FakeWeather");

            return new JsonResult(new
            {
                fromSomeWeb = "stuff from web controller",
                fromCore = coreResponseString,
                fromWeather = weatherResponseString
            });
        }
    }
}
