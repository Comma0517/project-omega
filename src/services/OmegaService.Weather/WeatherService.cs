using System;
using System.Reflection;
using EnvironmentSettings.Interface;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Omega.Plumbing;
using Omega.Plumbing.Data;
using OmegaService.Weather.Interface;
using OmegaService.Weather.Logic;

namespace OmegaService.Weather
{
    [DbInfo(DbName = "OmegaWeather")]
    public class WeatherService : ProjectOmegaService
    {
        public WeatherService(Assembly assembly) : base(assembly)
        {
        }

        public override void ConfigureServices(IServiceCollection services, ILogger logger, IEnvSettings envSettings)
        {
            base.ConfigureServices(services, logger, envSettings);
            Console.WriteLine("Yo dawg, initializing Weather P.O.S.");
            services.AddScoped<IFakeWeatherGetter, FakeWeatherGetter>();
        }
    }
}
