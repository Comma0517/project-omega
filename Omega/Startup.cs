using System;
using System.IO;
using EnvironmentSettings.Interface;
using EnvironmentSettings.Logic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Omega.Logic;

namespace Omega
{
    public class Startup
    {
        private readonly OmegaServiceRegistration _omegaServiceRegistration = new OmegaServiceRegistration();

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            SetupDotEnv();
            EnvHelper.Init();
            var envSettings = new EnvSettings(new EnvironmentVariableProvider());
            services.AddSingleton<IEnvSettings>(envSettings);

            _omegaServiceRegistration.LoadOmegaServices(services, Environment.GetEnvironmentVariable("SERVICE_KEY"), envSettings);
            
            services.AddControllers(); // We're letting the react app handle all views, so this is probably all we need.
        }
        
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            Console.WriteLine("**********");
            Console.WriteLine("**********");
            Console.WriteLine("ASPNETCORE_ENVIRONMENT: " + Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"));
            Console.WriteLine("**********");
            Console.WriteLine("**********");
            app.UseRouting();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });
            
            // Note that order matters here. OmegaService.Web registration of SPA resources fails if routing is not setup first.
            _omegaServiceRegistration.ConfigureOmegaServices(app, env);
        }

        private void SetupDotEnv()
        {
            var root = Directory.GetCurrentDirectory();
            var dotenv = Path.Combine(root, ".env");
            DotEnv.Load(dotenv);
        }
    }
}
