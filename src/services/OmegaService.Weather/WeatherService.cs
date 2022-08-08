using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using MikeyT.EnvironmentSettingsNS.Interface;
using Omega.Plumbing;
using Omega.Plumbing.Data;
using OmegaService.Weather.Interface;
using OmegaService.Weather.Logic;
using Serilog;

namespace OmegaService.Weather
{
    [DbInfo(DbName = "OmegaWeather")]
    public class WeatherService : ProjectOmegaService
    {
        public WeatherService(Assembly assembly) : base(assembly, Log.ForContext<WeatherService>())
        {
        }

        public override void ConfigureServices(IServiceCollection services, IEnvironmentSettings envSettings)
        {
            base.ConfigureServices(services, envSettings);
            _logger.Information("Initializing Weather POS");
            services.AddScoped<IFakeWeatherGetter, FakeWeatherGetter>();
        }
    }
}
