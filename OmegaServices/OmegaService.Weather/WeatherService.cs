using Microsoft.Extensions.DependencyInjection;
using OmegaPlumbing;
using OmegaService.Weather.Interface;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace OmegaService.Weather
{
    public class WeatherService : ProjectOmegaService
    {
        public WeatherService()
        {
        }

        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
        }

        public override void ConfigureServices(IServiceCollection services, ILogger logger)
        {
            logger.LogInformation("Yo dawg, initializing Weather P.O.S.");
            services.AddScoped<IFakeWeatherGetter, FakeWeatherGetter>();
        }
    }
}
