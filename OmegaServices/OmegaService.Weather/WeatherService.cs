using System;
using Microsoft.Extensions.DependencyInjection;
using OmegaPlumbing;
using OmegaService.Weather.Interface;

namespace OmegaService.Weather
{
    public class WeatherService : ProjectOmegaService
    {
        public WeatherService()
        {
        }
        
        public override void InitService(IServiceCollection services)
        {
            Console.WriteLine("You dawg, initializing Weather P.O.S.");
            services.AddScoped<IFakeWeatherGetter, FakeWeatherGetter>();
        }
    }
}
