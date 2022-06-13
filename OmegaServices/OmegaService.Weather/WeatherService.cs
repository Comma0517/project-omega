using EnvironmentSettings.Interface;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OmegaPlumbing;
using OmegaService.Weather.Interface;
using OmegaService.Weather.Logic;

namespace OmegaService.Weather
{
    public class WeatherService : ProjectOmegaService
    {
        public WeatherService()
        {
        }

        public override void ConfigureServices(IServiceCollection services, ILogger logger, IEnvSettings envSettings)
        {
            logger.LogInformation("Yo dawg, initializing Weather P.O.S.");
            services.AddScoped<IFakeWeatherGetter, FakeWeatherGetter>();
        }

        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
        }
    }
}
