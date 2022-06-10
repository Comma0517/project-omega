using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OmegaPlumbing;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using OmegaService.Web.Logic;
using System;

namespace OmegaService.Web
{
    public class WebService : ProjectOmegaService
    {
        public override void ConfigureServices(IServiceCollection services, ILogger logger)
        {
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "client-app/build";
            });
        }

        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            Log("=========");
            Log("CORE_HOST: " + EnvHelper.CORE_HOST);
            Log("=========");

            app.UseStaticFiles();

            app.UseSpaStaticFiles();

            // if (env.IsDevelopment())
            // {
            //     app.UseDeveloperExceptionPage();
            // }
            // else
            // {
            //     app.UseExceptionHandler("/Error");
            //     // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            //     // app.UseHsts();
            // }

            // app.UseHttpsRedirection();
            // app.UseStaticFiles();
            // if (EnvHelper.IS_WEB)
            // {
            //     app.UseSpaStaticFiles();
            // }

            // if (!env.IsDevelopment())
            // {
            //     app.UseSpa(spa =>
            //     {
            //         spa.Options.SourcePath = "client-app";

            //         if (env.IsDevelopment())
            //         {
            //             spa.UseReactDevelopmentServer(npmScript: "start");
            //         }
            //     });
            // }
        }

        private void Log(string message)
        {
            Console.WriteLine(message);
        }
    }
}