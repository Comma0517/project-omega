using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace Omega.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet("strings")]
        public IEnumerable<string> GetStrings()
        {
            return new string[] { "these", "are", "some", "other", "strings" };
        }

        [HttpGet("omega")]
        public async Task<IActionResult> OmegaTest()
        {
            string responseString;
            var handler = new HttpClientHandler
            {
                ClientCertificateOptions = ClientCertificateOption.Manual,
                ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => true
            };
            using (var client = new HttpClient(handler))
            {
                responseString = await client.GetStringAsync($"http://{EnvHelper.CORE_HOST}:{EnvHelper.CORE_PORT}/api/WeatherForecast/strings");
            }

            return Ok("stuff from gateway + stuff from other service instance: " + responseString);
        }
        
        [HttpGet("isWeb")]
        public IActionResult IsWeb()
        {
            return Ok(EnvHelper.IS_WEB);
        }
    }
}
