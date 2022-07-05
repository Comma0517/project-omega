using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace OmegaInterop.Weather
{
    public interface IWeatherClient
    {
        HttpClient GetHttpClient();
        Task<List<WeatherForecast>> GetRandomWeatherForecasts();
    }
}
