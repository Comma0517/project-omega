using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using OmegaInterop.Weather;
using OmegaService.Weather.Interface;
using RandomStuff;

namespace OmegaService.Weather.Logic
{
    public class FakeWeatherGetter : IFakeWeatherGetter
    {
        private readonly ILogger<FakeWeatherGetter> _logger;

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        public FakeWeatherGetter(ILogger<FakeWeatherGetter> logger)
        {
            _logger = logger;
        }

        public string GetRandomTempMessage()
        {
            _logger.LogInformation("Called GetRandomTempMessage in FakeWeatherGetter class");
            return $"It is {OmegaRandom.getRandomNumber(0, 120)} degrees fahrenheit in some alternate dimension";
        }

        public IEnumerable<WeatherForecast> GetRandomWeatherForecasts()
        {
            _logger.LogInformation("Called GetRandomWeatherForecasts in FakeWeatherGetter class");
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = rng.Next(-20, 55),
                    Summary = Summaries[rng.Next(Summaries.Length)]
                })
                .ToArray();
        }
    }
}
