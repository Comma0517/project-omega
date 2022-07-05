using System.Reflection;
using EnvironmentSettings.Interface;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Omega.Plumbing;
using Omega.Plumbing.Data;
using OmegaService.Core.Data;
using OmegaService.Core.Interface;
using OmegaService.Core.Logic;

namespace OmegaService.Core
{
    public class CoreService : ProjectOmegaService
    {
        public CoreService(Assembly assembly) : base(assembly)
        {
        }

        public override void ConfigureServices(IServiceCollection services, ILogger logger, IEnvSettings envSettings)
        {
            base.ConfigureServices(services, logger, envSettings);

            services.AddScoped<IOmegaUserLogic, OmegaUserLogic>();
            services.AddScoped<IConnectionStringProvider, ConnectionStringProvider>();
            services.AddScoped<IOmegaUserRepository, OmegaUserRepository>();
        }
    }
}
