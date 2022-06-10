using System.Linq;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using OmegaPlumbing;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace Omega.Logic
{
    public class OmegaServiceRegistration
    {
        private readonly ILogger _logger;
        private static readonly string OMEGA_SERVICE_PREFIX = "OmegaService";
        private static List<ProjectOmegaService> _omegaServices = new List<ProjectOmegaService>();

        public OmegaServiceRegistration()
        {
            // Unsure what the right way to setup logging during startup. It changed for .net core 3.x - research this.
            using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            _logger = loggerFactory.CreateLogger<OmegaServiceRegistration>();
        }

        public void LoadOmegaServices(IServiceCollection appServices)
        {
            _logger.LogInformation("\n-----------------------------\nRegistering Omega Services...\n");

            _omegaServices = LoadOmegaServices();

            foreach (var service in _omegaServices)
            {
                _logger.LogInformation("Calling InitService for type " + service.GetType().Name);
                service.ConfigureServices(appServices, _logger);
            }
            _logger.LogInformation("\n-----------------------------\n");
        }

        public void ConfigureOmegaServices(IApplicationBuilder app, IWebHostEnvironment env)
        {
            foreach (var omegaService in _omegaServices)
            {
                omegaService.Configure(app, env);
            }
        }

        // Note that we have to manually load assemblies that aren't explicitly used (we're only accessing these via reflection).
        //
        // May need to add functionality to dynamically load any assemblies each OmegaService references after manually loading
        // See https://dotnetstories.com/blog/Dynamically-pre-load-assemblies-in-a-ASPNET-Core-or-any-C-project-en-7155735300
        private List<ProjectOmegaService> LoadOmegaServices()
        {
            var omegaServices = new List<ProjectOmegaService>();

            var executingDir = AppDomain.CurrentDomain.BaseDirectory;
            var searchPattern = $"{OMEGA_SERVICE_PREFIX}*.dll";
            string[] filePaths = Directory.GetFiles(executingDir, searchPattern, SearchOption.TopDirectoryOnly);


            filePaths.ToList().ForEach(filePath =>
            {
                string omegaServiceAssemblyName = Path.GetFileName(filePath).Replace(".dll", "");
                _logger.LogInformation("Found OmegaService " + omegaServiceAssemblyName);
                Assembly assembly = Assembly.Load(omegaServiceAssemblyName);
                var omegaServiceTypes = assembly.GetTypes()
                    .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(ProjectOmegaService)))
                    .ToList();
                foreach (var omegaServiceType in omegaServiceTypes)
                {
                    omegaServices.Add((ProjectOmegaService)Activator.CreateInstance(omegaServiceType));
                }
            });

            return omegaServices;
        }
    }
}
