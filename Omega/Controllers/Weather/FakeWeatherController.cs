using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Weather;

namespace Omega.Controllers.Weather
{
    [ApiController]
    [Route("api/Weather/[controller]")]
    public class FakeWeatherController : ControllerBase
    {
        private readonly ILogger<FakeWeatherController> _logger;
        
        public FakeWeatherController(ILogger<FakeWeatherController> logger)
        {
            _logger = logger;
        }
        
        [HttpGet]
        public IActionResult GetFakeWeatherMessage()
        {
            var weatherGetter = new FakeWeatherGetter();
            return Ok(weatherGetter.GetRandomTempMessage());
        }
    }
}
