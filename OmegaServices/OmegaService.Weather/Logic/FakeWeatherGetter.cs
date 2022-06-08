using System;
using Microsoft.Extensions.Logging;
using RandomStuff;
using OmegaService.Weather.Interface;

namespace OmegaService.Weather
{
    public class FakeWeatherGetter : IFakeWeatherGetter
    {
        private readonly ILogger<FakeWeatherGetter> _logger;
        
        public FakeWeatherGetter(ILogger<FakeWeatherGetter> logger)
        {
            _logger = logger;
        }
        
        public string GetRandomTempMessage()
        {
            _logger.LogInformation($"Called GetRandomTempMessage in FakeWeatherGetter class");
            return $"It is {OmegaRandom.getRandomNumber(0, 120)} degrees fahrenheit in some alternate dimension";
        }
    }
}
