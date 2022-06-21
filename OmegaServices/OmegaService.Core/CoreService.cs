using EnvironmentSettings.Interface;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Omega.Plumbing;
using OmegaService.Core.Interface;
using OmegaService.Core.Logic;

namespace OmegaService.Core
{
    [DbInfo(DbName = "OmegaCore")]
    public class CoreService : ProjectOmegaService
    {
        public override void ConfigureServices(IServiceCollection services, ILogger logger, IEnvSettings envSettings)
        {
            services.AddScoped<IOmegaUserLogic, OmegaUserLogic>();
        }
        
        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
        }
    }
}
