using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OmegaPlumbing;

namespace OmegaService.Auth
{
    public class AuthService : ProjectOmegaService
    {
        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            throw new NotImplementedException();
        }

        public override void ConfigureServices(IServiceCollection services, ILogger logger)
        {
        }
    }
}
