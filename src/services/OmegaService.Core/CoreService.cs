using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using MikeyT.EnvironmentSettingsNS.Interface;
using Omega.Plumbing;
using Omega.Plumbing.Data;
using OmegaService.Core.Data;
using OmegaService.Core.Interface;
using OmegaService.Core.Logic;
using Serilog;

namespace OmegaService.Core
{
    public class CoreService : ProjectOmegaService
    {
        public CoreService(Assembly assembly) : base(assembly, Log.ForContext<CoreService>())
        {
        }

        public override void ConfigureServices(IServiceCollection services, IEnvironmentSettings envSettings)
        {
            base.ConfigureServices(services, envSettings);

            services.AddScoped<IOmegaUserLogic, OmegaUserLogic>();
            services.AddScoped<IConnectionStringProvider, ConnectionStringProvider>();
            services.AddScoped<IOmegaUserRepository, OmegaUserRepository>();
        }
    }
}
