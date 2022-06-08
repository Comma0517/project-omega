using System.Linq;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using OmegaPlumbing;
using System.Reflection;

namespace Omega.Logic
{
    public class OmegaServiceRegistration
    {
        private readonly ILogger _logger;

        public OmegaServiceRegistration()
        {
            // Unsure what the right way to setup logging during startup. It changed for .net core 3.x - research this.
            using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            _logger = loggerFactory.CreateLogger<OmegaServiceRegistration>();
        }

        public void InitServices()
        {
            _logger.LogInformation("\n-----------------------------\nRegistering Omega Services...\n");
            var services = new List<ProjectOmegaService>();

            var omegaServiceAssemblies = typeof(OmegaServiceRegistration).Assembly.GetReferencedAssemblies()
                .Where(a => a.Name.StartsWith("OmegaService."))
                .ToList();
            omegaServiceAssemblies.ForEach(a => Console.WriteLine(a.Name));

            foreach (var assemblyName in omegaServiceAssemblies)
            {
                var omegaServiceTypes = Assembly.Load(assemblyName).GetTypes()
                    .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(ProjectOmegaService)))
                    .ToList();
                _logger.LogInformation($"Number of ProjectOmegaService types in assembly {assemblyName.Name}: {omegaServiceTypes.Count}");

                foreach (var omegaServiceType in omegaServiceTypes)
                {
                    services.Add((ProjectOmegaService)Activator.CreateInstance(omegaServiceType));
                }
            }

            foreach (var service in services)
            {
                _logger.LogInformation("Calling InitService for type " + service.GetType().Name);
                service.InitService();
            }
            _logger.LogInformation("\n-----------------------------\n");
        }
    }
}
