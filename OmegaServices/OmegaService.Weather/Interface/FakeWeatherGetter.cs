using System.Collections.Generic;
using OmegaModel.Weather;

namespace OmegaService.Weather.Interface
{
    public interface IFakeWeatherGetter
    {
        string GetRandomTempMessage();
        IEnumerable<WeatherForecast> GetRandomWeatherForecasts();
    }
}
