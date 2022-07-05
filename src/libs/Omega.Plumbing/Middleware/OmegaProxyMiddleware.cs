using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnvironmentSettings.Interface;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Omega.Plumbing.Http;

namespace Omega.Plumbing.Middleware
{
    public class OmegaProxyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly OmegaServiceRegistration _omegaServiceRegistration;
        private readonly IEnvSettings _envSettings;
        private readonly List<string> _urlPrefixesToProxy;

        public OmegaProxyMiddleware(
            RequestDelegate next,
            OmegaServiceRegistration omegaServiceRegistration,
            IEnvSettings envSettings,
            List<string> urlPrefixesToProxy)
        {
            _next = next;
            _omegaServiceRegistration = omegaServiceRegistration;
            _envSettings = envSettings;
            _urlPrefixesToProxy = urlPrefixesToProxy;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            Console.WriteLine("*****************");
            Console.WriteLine("URL: " + context.Request.GetDisplayUrl());

            if (!context.RequestPathStartsWith("/api/"))
            {
                Console.WriteLine("Does not start with /api/, calling next()");
                await _next(context);
            }
            else
            {
                if (ShouldProxyRequest(context))
                {
                    Console.WriteLine("Proxy request: " + context.Request.GetDisplayUrl());
                    await context.ProxyRequest(_omegaServiceRegistration.GetProxyUri(new Uri(context.Request.GetEncodedUrl()), _envSettings));
                    return;
                }

                await _next(context);
            }
        }

        private bool ShouldProxyRequest(HttpContext context)
        {
            return !context.Request.Headers.ContainsKey(OmegaGlobalConstants.INTER_SERVICE_HEADER_KEY)
                   && _urlPrefixesToProxy.Any(context.RequestPathStartsWith);
        }
    }

    public static class OmegaProxyMiddlewareExtensions
    {
        public static IApplicationBuilder UseOmegaProxy(
            this IApplicationBuilder builder,
            OmegaServiceRegistration omegaServiceRegistration,
            IEnvSettings envSettings,
            List<string> urlPrefixesToProxy)
        {
            return builder.UseMiddleware<OmegaProxyMiddleware>(omegaServiceRegistration, envSettings, urlPrefixesToProxy);
        }
    }
}
