using System;
using System.Reflection;
using EnvironmentSettings.Interface;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Omega.Plumbing
{
    public abstract class ProjectOmegaService
    {
        public Assembly Assembly { get; set; }
        
        // Wire up dependency injection
        public abstract void ConfigureServices(IServiceCollection services, ILogger logger, IEnvSettings envSettings);
        
        // Setup DB connections, queue init, etc
        public abstract void Configure(IApplicationBuilder app, IWebHostEnvironment env);
        
        public abstract void ConfigureEndpoints(IEndpointRouteBuilder endpoints, IApplicationBuilder app, IWebHostEnvironment env);

        // Currently only used by Web to configure Spa services
        public abstract void ConfigureLast(IApplicationBuilder app, IWebHostEnvironment env);

        protected static void LogEnvSettings(IEnvSettings envSettings, string serviceKey)
        {
            if (serviceKey != null && envSettings.GetString(GlobalSettings.SERVICE_KEY, null) == serviceKey)
            {
                
                Console.WriteLine(OmegaGlobalConstants.LOG_LINE_SEPARATOR + envSettings.GetAllAsSafeLogString());                
            }
        }
    }   
}
