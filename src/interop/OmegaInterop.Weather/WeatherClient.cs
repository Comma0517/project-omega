using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Omega.Plumbing.Http;

namespace OmegaInterop.Weather
{
    public class WeatherClient : OmegaHttpClient, IWeatherClient
    {
        public WeatherClient(HttpClient httpClient) : base(httpClient)
        {
        }

        public async Task<List<WeatherForecast>> GetRandomWeatherForecasts()
        {
            return await _httpClient.GetFromJsonAsync<List<WeatherForecast>>("api/Weather/WeatherForecast/Randoms");
        }
    }
}
