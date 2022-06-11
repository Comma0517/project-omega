using EnvironmentSettings.Interface;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace OmegaPlumbing
{
    public abstract class ProjectOmegaService
    {
        // Wire up dependency injection
        public abstract void ConfigureServices(IServiceCollection services, ILogger logger, IEnvSettings envSettings);
        
        // Setup DB connections, queue init, etc
        public abstract void Configure(IApplicationBuilder app, IWebHostEnvironment env);
    }   
}
