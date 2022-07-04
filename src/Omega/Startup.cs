using System;
using EnvironmentSettings.Interface;
using EnvironmentSettings.Logic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Omega.Plumbing;
using Omega.Utils;

namespace Omega
{
    public class Startup
    {
        private readonly OmegaServiceRegistration _omegaServiceRegistration = new();
        private string _serviceKey;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            DotEnv.Load();
            var envSettings = new EnvSettings(new EnvironmentVariableProvider());
            envSettings.AddSettings<GlobalSettings>();
            services.AddSingleton<IEnvSettings>(envSettings);

            _serviceKey = envSettings.GetString(GlobalSettings.SERVICE_KEY, null);
            if (_serviceKey == null)
            {
                Console.Write($"{OmegaGlobalConstants.LOG_LINE_SEPARATOR}{envSettings.GetAllAsSafeLogString()}");
            }

            _omegaServiceRegistration.LoadOmegaServices(_serviceKey);
            _omegaServiceRegistration.InitOmegaServices(services, envSettings);

            services.AddControllers(); // We're letting the react app handle all views, so this is probably all we need.
        }

        // Note that order matters here. OmegaService.Web registration of SPA resources fails if routing and endpoints not setup first.
        // Most middleware setup happens between routing and endpoints definition where endpoints are non-null (httpContext.GetEndpoint()).
        // UseEndpoints is terminal if a route matches. This means that a response is returned, so no more middleware after this will run.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            _omegaServiceRegistration.ConfigureOmegaServices(app, env);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
                _omegaServiceRegistration.ConfigureOmegaServicesEndpoints(endpoints, app, env);
            });

            _omegaServiceRegistration.ConfigureLastOmegaServices(app, env);
        }
    }
}
