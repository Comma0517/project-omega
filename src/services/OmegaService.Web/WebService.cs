using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MikeyT.EnvironmentSettingsNS.Interface;
using Omega.Plumbing;
using OmegaInterop.Core;
using OmegaInterop.Weather;
using Serilog;

namespace OmegaService.Web
{
    public class WebService : ProjectOmegaService
    {
        public WebService(Assembly assembly) : base(assembly, Log.ForContext<WebService>())
        {
        }

        public override void ConfigureServices(IServiceCollection services, IEnvironmentSettings envSettings)
        {
            base.ConfigureServices(services, envSettings);
            envSettings.AddSettings<WebEnvSettings>();
            services.AddSpaStaticFiles(configuration => { configuration.RootPath = "client-app/build"; });

            services.AddOmegaHttpClient<IWeatherClient, WeatherClient>("Weather", envSettings);
            services.AddOmegaHttpClient<ICoreClient, CoreClient>("Core", envSettings);
        }

        public override void ConfigureBeforeRouting(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseStaticFiles();

            app.UseSpaStaticFiles();

            // if (!env.IsDevelopment())
            // {
            //     // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            //     // app.UseHsts();
            // }

            // app.UseHttpsRedirection();
        }

        public override void ConfigureLast(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSpa(spa => { spa.Options.SourcePath = "client-app"; });
        }
    }
}
