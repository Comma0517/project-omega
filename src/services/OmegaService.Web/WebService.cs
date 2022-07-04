using EnvironmentSettings.Interface;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Omega.Plumbing;

namespace OmegaService.Web
{
    public class WebService : ProjectOmegaService
    {
        public override void ConfigureServices(IServiceCollection services, ILogger logger, IEnvSettings envSettings)
        {
            LogEnvSettings(envSettings, "Web");
            envSettings.AddSettings<WebEnvSettings>();
            services.AddSpaStaticFiles(configuration => { configuration.RootPath = "client-app/build"; });
        }

        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        { 
            // if (!env.IsDevelopment())
            // {
            //     // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            //     // app.UseHsts();
            // }

            // app.UseHttpsRedirection();
        }

        public override void ConfigureEndpoints(IEndpointRouteBuilder endpoints, IApplicationBuilder app, IWebHostEnvironment env)
        {
        }

        public override void ConfigureLast(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseStaticFiles();
            
            app.UseSpaStaticFiles();
            
            app.UseSpa(spa => { spa.Options.SourcePath = "client-app"; });
        }
    }
}
