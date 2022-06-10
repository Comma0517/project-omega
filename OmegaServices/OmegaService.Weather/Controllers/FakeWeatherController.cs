using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OmegaService.Weather.Interface;

namespace OmegaService.Weather.Controllers
{
    [ApiController]
    [Route("api/Weather/[controller]")]
    public class FakeWeatherController : ControllerBase
    {
        private readonly ILogger<FakeWeatherController> _logger;
        private readonly IFakeWeatherGetter _fakeWeatherGetter;
        
        public FakeWeatherController(ILogger<FakeWeatherController> logger, IFakeWeatherGetter fakeWeatherGetter)
        {
            _logger = logger;
            _fakeWeatherGetter = fakeWeatherGetter;
        }
        
        [HttpGet]
        public IActionResult GetFakeWeatherMessage()
        {
            return Ok(_fakeWeatherGetter.GetRandomTempMessage());
        }
    }
}
