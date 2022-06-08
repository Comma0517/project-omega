using System;
using Microsoft.Extensions.DependencyInjection;

namespace OmegaPlumbing
{
    public abstract class ProjectOmegaService
    {
        // Setup DB connection
        // Setup Dependency Injection
        // etc
        public abstract void InitService(IServiceCollection services);
    }
}
