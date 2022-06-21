using EnvironmentSettings.Interface;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Omega.Plumbing;

namespace OmegaService.Web
{
    public class WebService : ProjectOmegaService
    {
        public override void ConfigureServices(IServiceCollection services, ILogger logger, IEnvSettings envSettings)
        {
            envSettings.AddSettings<WebEnvSettings>();
            logger.LogInformation(envSettings.GetAllAsSafeLogString());
            services.AddSpaStaticFiles(configuration => { configuration.RootPath = "client-app/build"; });
        }

        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseStaticFiles();

            app.UseSpaStaticFiles();

            app.UseSpa(spa => { spa.Options.SourcePath = "client-app"; });

            // if (!env.IsDevelopment())
            // {
            //     // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            //     // app.UseHsts();
            // }

            // app.UseHttpsRedirection();
        }
    }
}
