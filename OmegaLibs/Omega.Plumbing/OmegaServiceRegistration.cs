using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using EnvironmentSettings.Interface;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Omega.Plumbing
{
    public class OmegaServiceRegistration
    {
        private readonly ILogger _logger;
        private List<ProjectOmegaService> _omegaServices = new();
        
        private static readonly string OMEGA_SERVICE_PREFIX = "OmegaService";

        public OmegaServiceRegistration()
        {
            using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            _logger = loggerFactory.CreateLogger<OmegaServiceRegistration>();
        }

        // Note that we have to manually load assemblies that aren't explicitly used (we're only accessing these via reflection).
        // May need to add functionality to dynamically load references of references - see
        // https://dotnetstories.com/blog/Dynamically-pre-load-assemblies-in-a-ASPNET-Core-or-any-C-project-en-7155735300
        public List<ProjectOmegaService> LoadOmegaServices(string serviceKey)
        {
            Console.WriteLine("\n-----------------------------\nLoading Omega Services...\n");
            var omegaServices = new List<ProjectOmegaService>();

            var executingDir = AppDomain.CurrentDomain.BaseDirectory;
            var searchPattern = $"{OMEGA_SERVICE_PREFIX}*.dll";
            string[] filePaths = Directory.GetFiles(executingDir, searchPattern, SearchOption.TopDirectoryOnly);

            foreach (var filePath in filePaths)
            {
                var omegaServiceAssemblyName = Path.GetFileName(filePath).Replace(".dll", "");

                if (serviceKey != null && !omegaServiceAssemblyName.EndsWith(serviceKey))
                {
                    continue;
                }

                Console.WriteLine("Found OmegaService " + omegaServiceAssemblyName);

                var assembly = Assembly.Load(omegaServiceAssemblyName);
                

                var omegaServiceTypes = assembly.GetTypes()
                    .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(ProjectOmegaService)))
                    .ToList();

                foreach (var omegaServiceType in omegaServiceTypes)
                {
                    var instance = (ProjectOmegaService) Activator.CreateInstance(omegaServiceType);
                    instance.Assembly = assembly;
                    omegaServices.Add(instance);
                }
            }

            _omegaServices = omegaServices;

            Console.WriteLine("\n-----------------------------\n");

            return _omegaServices;
        }

        public void InitOmegaServices(IServiceCollection appServices, string serviceKey, IEnvSettings envSettings)
        {
            Console.WriteLine("\n-----------------------------\nInitializing Omega Services...\n");

            foreach (var service in _omegaServices)
            {
                Console.WriteLine("Calling InitService for type " + service.GetType().Name);
                service.ConfigureServices(appServices, _logger, envSettings);
            }

            Console.WriteLine("\n-----------------------------\n");
        }

        public void ConfigureOmegaServices(IApplicationBuilder app, IWebHostEnvironment env)
        {
            foreach (var omegaService in _omegaServices)
            {
                Console.WriteLine("Configuring service: " + omegaService.GetType().Name);
                omegaService.Configure(app, env);
            }
        }
    }
}