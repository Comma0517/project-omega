using System;
using EnvironmentSettings.Interface;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Omega.Plumbing;
using OmegaService.Core.Data;
using OmegaService.Core.Interface;
using OmegaService.Core.Logic;

namespace OmegaService.Core
{
    public class CoreService : ProjectOmegaService
    {
        public override void ConfigureServices(IServiceCollection services, ILogger logger, IEnvSettings envSettings)
        {
            Console.WriteLine(envSettings.GetAllAsSafeLogString());
            services.AddScoped<IOmegaUserLogic, OmegaUserLogic>();
            services.AddScoped<IConnectionStringProvider, ConnectionStringProvider>();
            services.AddScoped<IOmegaUserRepository, OmegaUserRepository>();
        }
        
        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
        }
    }
}
