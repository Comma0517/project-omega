using System.Reflection;
using EnvironmentSettings.Interface;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
    }   
}
