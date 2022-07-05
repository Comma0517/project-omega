using System.Collections.Generic;
using OmegaInterop.Weather;

namespace OmegaService.Weather.Interface
{
    public interface IFakeWeatherGetter
    {
        string GetRandomTempMessage();
        IEnumerable<WeatherForecast> GetRandomWeatherForecasts();
    }
}
