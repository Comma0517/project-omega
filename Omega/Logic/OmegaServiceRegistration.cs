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
            using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            _logger = loggerFactory.CreateLogger<OmegaServiceRegistration>();
        }

        public void InitServices()
        {
            _logger.LogInformation("Registering Omega Services...");
            var services = new List<ProjectOmegaService>();
            
            // We could scan all assemplies and find
            // AppDomain.CurrentDomain.GetAssemblies().Select(a => a.FullName).ToList().ForEach(name => Console.WriteLine(name));
            Assembly ass = typeof(OmegaServiceRegistration).Assembly;
            // ass.GetReferencedAssemblies().Select(a => a.FullName).ToList().ForEach(name => Console.WriteLine(name));
            var assemblies = ass.GetReferencedAssemblies()
                .Where(a => !a.FullName.StartsWith("Microsoft.") && !a.FullName.StartsWith("System."))
                .ToList();
            assemblies.ForEach(a => Console.WriteLine(a.FullName));

            foreach (var service in services)
            {
                
            }
        }
    }
}
