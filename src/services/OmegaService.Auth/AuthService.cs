using EnvironmentSettings.Interface;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Omega.Plumbing;

namespace OmegaService.Auth
{
    public class AuthService : ProjectOmegaService
    {
        public override void ConfigureServices(IServiceCollection services, ILogger logger, IEnvSettings envSettings)
        {
            LogEnvSettings(envSettings, "Auth");
        }

        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
        }
        
        public override void ConfigureEndpoints(IEndpointRouteBuilder endpoints, IApplicationBuilder app, IWebHostEnvironment env)
        {
        }
        
        public override void ConfigureLast(IApplicationBuilder app, IWebHostEnvironment env)
        {
        }
    }
}
