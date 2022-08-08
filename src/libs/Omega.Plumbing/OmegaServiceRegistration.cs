using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using MikeyT.EnvironmentSettingsNS.Interface;
using Serilog;

namespace Omega.Plumbing
{
    public class OmegaServiceRegistration
    {
        private readonly ILogger _logger;
        private List<ProjectOmegaService> _omegaServices = new();
        private readonly List<string> _allServiceKeys = new();

        public OmegaServiceRegistration()
        {
            _logger = Log.ForContext<OmegaServiceRegistration>();
        }

        // Note that we have to manually load assemblies that aren't explicitly used (we're only accessing these via reflection).
        // May need to add functionality to dynamically load references of references - see
        // https://dotnetstories.com/blog/Dynamically-pre-load-assemblies-in-a-ASPNET-Core-or-any-C-project-en-7155735300
        public IList<ProjectOmegaService> LoadOmegaServices(string forServiceKey)
        {
            _logger.Information(OmegaGlobalConstants.LOG_LINE_SEPARATOR + "Loading Omega Services...\n");
            var omegaServices = new List<ProjectOmegaService>();

            var executingDir = AppDomain.CurrentDomain.BaseDirectory;
            var searchPattern = $"{OmegaGlobalConstants.OMEGA_SERVICE_PREFIX}*.dll";
            string[] filePaths = Directory.GetFiles(executingDir, searchPattern, SearchOption.TopDirectoryOnly);

            foreach (var filePath in filePaths)
            {
                var omegaServiceAssemblyName = Path.GetFileName(filePath).Replace(".dll", "");
                var serviceKey = ProjectOmegaService.GetServiceKeyFromAssemblyName(omegaServiceAssemblyName);
                _allServiceKeys.Add(serviceKey);
                
                if (forServiceKey != null && !omegaServiceAssemblyName.EndsWith(forServiceKey))
                {
                    continue;
                }

                _logger.Information("Found OmegaService {OmegaServiceName}", omegaServiceAssemblyName);

                var assembly = Assembly.Load(omegaServiceAssemblyName);

                var omegaServiceTypes = assembly.GetTypes()
                    .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(ProjectOmegaService)))
                    .ToList();

                foreach (var omegaServiceType in omegaServiceTypes)
                {
                    var instance = (ProjectOmegaService) Activator.CreateInstance(omegaServiceType, assembly);
                    if (instance == null)
                    {
                        throw new ApplicationException("Instantiating Project Omega Service resulted in null for type " + omegaServiceType.Name);
                    }

                    omegaServices.Add(instance);
                }
            }

            _omegaServices = omegaServices;

            return _omegaServices;
        }

        public void InitOmegaServices(IServiceCollection appServices, IEnvironmentSettings envSettings)
        {
            _logger.Information(OmegaGlobalConstants.LOG_LINE_SEPARATOR + "Initializing Omega Services...\n");

            foreach (var service in _omegaServices)
            {
                _logger.Information("Calling InitService for type {OmegaServiceType}", service.GetType().Name);
                service.ConfigureServices(appServices, envSettings);
            }
        }
        
        public void ConfigureBeforeRouting(IApplicationBuilder app, IWebHostEnvironment env)
        {
            _logger.Information(OmegaGlobalConstants.LOG_LINE_SEPARATOR + "Configuring services before routing...\n");

            foreach (var omegaService in _omegaServices)
            {
                _logger.Information("Configuring service middlewares for: {OmegaServiceType}", omegaService.GetType().Name);
                omegaService.ConfigureBeforeRouting(app, env);
            }
        }

        public void ConfigureMiddlewareHooks(IApplicationBuilder app, IWebHostEnvironment env)
        {
            _logger.Information(OmegaGlobalConstants.LOG_LINE_SEPARATOR + "Configuring service middlewares...\n");

            foreach (var omegaService in _omegaServices)
            {
                _logger.Information("Configuring service middlewares for: {OmegaServiceType}", omegaService.GetType().Name);
                omegaService.ConfigureMiddleware(app, env);
            }
        }

        public void ConfigureEndpoints(IEndpointRouteBuilder endpoints, IApplicationBuilder app, IWebHostEnvironment env)
        {
            _logger.Information(OmegaGlobalConstants.LOG_LINE_SEPARATOR + "Configuring endpoints...\n");

            foreach (var omegaService in _omegaServices)
            {
                _logger.Information("Configuring endpoints for: {OmegaServiceType}", omegaService.GetType().Name);
                omegaService.ConfigureEndpoints(endpoints, app, env);
            }
        }

        public void ConfigureLastChanceHooks(IApplicationBuilder app, IWebHostEnvironment env)
        {
            _logger.Information(OmegaGlobalConstants.LOG_LINE_SEPARATOR + "Configuring last chance hooks...\n");

            foreach (var omegaService in _omegaServices)
            {
                _logger.Information("Configuring last chance hooks for: {OmegaServiceType}", omegaService.GetType().Name);
                omegaService.ConfigureLast(app, env);
            }

            _logger.Information(OmegaGlobalConstants.LOG_LINE_SEPARATOR);
        }

        public IList<string> GetAllServiceKeys()
        {
            return _allServiceKeys;
        }
        
        public Uri GetProxyUri(Uri originalUri, IEnvironmentSettings envSettings)
        {
            _logger.Debug("Proxy original path: {OriginalPath}", originalUri);
            if (originalUri == null)
            {
                throw new ApplicationException("Cannot get proxy Uri from a null Uri");
            }

            var noServiceFoundError = "Service not found for Uri " + originalUri.ToString();
            if (originalUri.Segments.Length < 3)
            {
                throw new ApplicationException(noServiceFoundError);
            }

            // Example segments: /api/Core/SomeEndpoint, [0] = /, [1] = api/, [2] = Core/
            var serviceKeyUpper = originalUri.Segments[2].Replace("/", "").ToUpper();
            var host = envSettings.GetString($"{serviceKeyUpper}_HOST");
            var port = envSettings.GetString($"{serviceKeyUpper}_PORT");

            var uriBuilder = new UriBuilder(originalUri) {Host = host, Port = int.Parse(port)};
            _logger.Debug("Proxy new path: {NewPath}", uriBuilder.Path);
            return uriBuilder.Uri;
        }
    }
}
