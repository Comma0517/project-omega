using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MikeyT.EnvironmentSettingsNS.Interface;
using MikeyT.EnvironmentSettingsNS.Logic;
using Omega.Plumbing;
using Omega.Plumbing.Http;
using Omega.Plumbing.Middleware;
using Serilog;

namespace Omega
{
    public class Startup
    {
        private IEnvironmentSettings _envSettings;
        private readonly OmegaServiceRegistration _omegaServiceRegistration = new();
        private string _serviceKey;
        private List<string> _allNonWebServiceUrlPrefixes;
        private ILogger _logger;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        // ReSharper disable once MemberCanBePrivate.Global
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            _logger = Log.ForContext<Startup>();

            var envSettings = new EnvironmentSettings(new EnvironmentVariableProvider());
            envSettings.AddSettings<GlobalSettings>();
            _envSettings = envSettings;

            services.AddSingleton<IEnvironmentSettings>(envSettings);

            _serviceKey = envSettings.GetString(GlobalSettings.SERVICE_KEY, null);
            if (_serviceKey == null)
            {
                Log.Information("Environment settings loaded:\n{EnvSettings}", envSettings.GetAllAsSafeLogString());
            }

            _omegaServiceRegistration.LoadOmegaServices(_serviceKey);
            _omegaServiceRegistration.InitOmegaServices(services, envSettings);

            services.AddProxy(options =>
            {
                options.PrepareRequest = (_, message) =>
                {
                    message.Headers.Add(OmegaGlobalConstants.INTER_SERVICE_HEADER_KEY, "true");
                    return Task.FromResult(0);
                };
            });

            services.AddControllers(); // We're letting the react app handle all views, so this is probably all we need instead of AddControllersWithViews.
        }

        // Note that order matters here. OmegaService.Web registration of SPA resources fails if routing and endpoints not setup first.
        // Most middleware setup happens between routing and endpoints definition where endpoints are non-null (httpContext.GetEndpoint()).
        // UseEndpoints is terminal if a route matches. This means that a response is returned, so no more middleware after this will run.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            PopulateAllServiceUrlPrefixes();
            
            if (!env.IsDevelopment())
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                // app.UseHsts();
            }

            app.UseHttpsRedirection();

            _omegaServiceRegistration.ConfigureBeforeRouting(app, env);

            app.UseRouting();

            _omegaServiceRegistration.ConfigureMiddlewareHooks(app, env);

            app.UseOmegaProxy(_omegaServiceRegistration, _envSettings, _allNonWebServiceUrlPrefixes);

            app.UseEndpoints(endpoints =>
            {
                _omegaServiceRegistration.ConfigureEndpoints(endpoints, app, env);
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            // Catch-all for not found routes. Requests that aren't for the SPA or any API endpoint.
            app.Use(next => async context =>
            {
                if (context.RequestPathStartsWith("/api/"))
                {
                    _logger.Information("Catch-all route reached for non-web requests with no known api endpoint - returning 404 for {NotFoundUrl}",
                        context.Request.GetDisplayUrl());
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    await context.Response.WriteAsync("Not Found");
                    return;
                }

                await next(context);
            });

            _omegaServiceRegistration.ConfigureLastChanceHooks(app, env);
        }

        private void PopulateAllServiceUrlPrefixes()
        {
            _allNonWebServiceUrlPrefixes = new List<string>();
            foreach (var key in _omegaServiceRegistration.GetAllServiceKeys().Where(sk => sk != "Web"))
            {
                _allNonWebServiceUrlPrefixes.Add($"/api/{key}/");
            }
        }
    }
}
