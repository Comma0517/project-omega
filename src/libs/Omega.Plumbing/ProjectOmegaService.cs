using System;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using MikeyT.EnvironmentSettingsNS.Interface;
using Serilog;

namespace Omega.Plumbing
{
    public abstract class ProjectOmegaService
    {
        protected readonly ILogger _logger;
        
        // Convention based key. Example: for assembly OmegaService.Auth the ServiceKey is Auth.
        private string ServiceKey { get; }

        // Currently used to infer the ServiceKey by convention and for DbMigrator to load scripts embedded in the assembly
        public Assembly Assembly { get; }

        protected ProjectOmegaService(Assembly assembly, ILogger logger)
        {
            Assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            var assemblyName = assembly.GetName().Name;

            if (assemblyName == null)
            {
                throw new ApplicationException("Project Omega Service assembly name is null for " + assembly.FullName);
            }

            ServiceKey = GetServiceKeyFromAssemblyName(assemblyName);
        }

        public static string GetServiceKeyFromAssemblyName(string assemblyName)
        {
            return assemblyName.Replace(OmegaGlobalConstants.OMEGA_SERVICE_PREFIX, "").Replace(".dll", "");
        }

        // Opportunity to wire up dependency injection
        public virtual void ConfigureServices(IServiceCollection services, IEnvironmentSettings envSettings)
        {
            LogEnvSettings(envSettings, ServiceKey);
        }

        // Web uses this to call UseStaticFiles and UseSpaStaticFiles before Startup calls UseRouting
        public virtual void ConfigureBeforeRouting(IApplicationBuilder app, IWebHostEnvironment env)
        {
        }

        public virtual void ConfigureMiddleware(IApplicationBuilder app, IWebHostEnvironment env)
        {
        }

        public virtual void ConfigureEndpoints(IEndpointRouteBuilder endpoints, IApplicationBuilder app, IWebHostEnvironment env)
        {
        }

        // Currently only used by Web to call UseSpa
        public virtual void ConfigureLast(IApplicationBuilder app, IWebHostEnvironment env)
        {
        }

        private void LogEnvSettings(IEnvironmentSettings envSettings, string serviceKey)
        {
            if (serviceKey != null && envSettings.GetString(GlobalSettings.SERVICE_KEY, null) == serviceKey)
            {
                _logger.Information("Environment settings loaded:\n{EnvSettings}", envSettings.GetAllAsSafeLogString());
            }
        }
    }

    public static class ProjectOmegaExtensions
    {
        public static void AddOmegaHttpClient<TClient, TImplementation>(this IServiceCollection services, string serviceKey, IEnvironmentSettings envSettings)
            where TClient : class
            where TImplementation : class, TClient
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (string.IsNullOrWhiteSpace(serviceKey))
            {
                throw new ArgumentNullException(nameof(serviceKey));
            }
            
            // Service HttpClient already registered
            if (services.Any(x => x.ServiceType == typeof(TClient)))
            {
                return;
            }

            services.AddHttpClient<TClient, TImplementation>(client =>
            {
                var hostEnvKey = $"{serviceKey.ToUpper()}_HOST";
                var portEnvKey = $"{serviceKey.ToUpper()}_PORT";
                client.BaseAddress = new Uri($"https://{envSettings.GetString(hostEnvKey)}:{envSettings.GetString(portEnvKey)}/");
                client.DefaultRequestHeaders.Add(OmegaGlobalConstants.INTER_SERVICE_HEADER_KEY, "true");
            }).ConfigureHttpMessageHandlerBuilder(builder =>
            {
                builder.PrimaryHandler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (m, c, ch, e) => true
                };
            });
        }
    }
}
