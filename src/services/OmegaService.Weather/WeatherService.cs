using System;
using EnvironmentSettings.Interface;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Omega.Plumbing;
using OmegaService.Weather.Interface;
using OmegaService.Weather.Logic;

namespace OmegaService.Weather
{
    [DbInfo(DbName = "OmegaWeather")]
    public class WeatherService : ProjectOmegaService
    {
        public WeatherService()
        {
        }

        public override void ConfigureServices(IServiceCollection services, ILogger logger, IEnvSettings envSettings)
        {
            LogEnvSettings(envSettings, "Weather");
            Console.WriteLine("Yo dawg, initializing Weather P.O.S.");
            services.AddScoped<IFakeWeatherGetter, FakeWeatherGetter>();
        }

        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
        }

        public override void ConfigureEndpoints(IEndpointRouteBuilder endpoints, IApplicationBuilder app, IWebHostEnvironment env)
        {
        }

        public override void ConfigureLast(IApplicationBuilder app, IWebHostEnvironment env)
        {
        }
    }
}
