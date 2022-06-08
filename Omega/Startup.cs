using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Omega.Logic;
using OmegaService.Weather;
using OmegaService.Weather.Interface;

namespace Omega
{
    public class Startup
    {
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
            new OmegaServiceRegistration().InitServices();

            services.AddControllersWithViews();

            services.AddScoped<IFakeWeatherGetter, FakeWeatherGetter>();

            // In production, the React files will be served from this directory
            if (EnvHelper.IS_WEB)
            {
                services.AddSpaStaticFiles(configuration =>
                {
                    configuration.RootPath = "client-app/build";
                });
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            Console.WriteLine($"IsDevelopment: {env.IsDevelopment()}");
            Console.WriteLine($"TEST_VAR: {Environment.GetEnvironmentVariable("TEST_VAR") ?? "null"}");
            Console.WriteLine($"DOT_ENV_TEST_VAR: {Environment.GetEnvironmentVariable("DOT_ENV_TEST_VAR") ?? "null"}");
            Console.WriteLine($"CORE_HOST: {EnvHelper.CORE_HOST}");
            Console.WriteLine($"CORE_PORT: {EnvHelper.CORE_PORT}");
            Console.WriteLine($"IS_WEB: {EnvHelper.IS_WEB}");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                // app.UseHsts();
            }

            // app.UseHttpsRedirection();
            app.UseStaticFiles();
            if (EnvHelper.IS_WEB)
            {
                app.UseSpaStaticFiles();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            if (!env.IsDevelopment() && EnvHelper.IS_WEB)
            {
                app.UseSpa(spa =>
                {
                    spa.Options.SourcePath = "client-app";

                    if (env.IsDevelopment())
                    {
                        spa.UseReactDevelopmentServer(npmScript: "start");
                    }
                });
            }
        }

        private void SetupDotEnv()
        {
            var root = Directory.GetCurrentDirectory();
            var dotenv = Path.Combine(root, ".env");
            DotEnv.Load(dotenv);
        }
    }
}
