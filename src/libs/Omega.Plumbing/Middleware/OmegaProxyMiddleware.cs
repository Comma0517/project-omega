using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using MikeyT.EnvironmentSettingsNS.Interface;
using Omega.Plumbing.Http;
using Serilog;

namespace Omega.Plumbing.Middleware
{
    public class OmegaProxyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly OmegaServiceRegistration _omegaServiceRegistration;
        private readonly IEnvironmentSettings _envSettings;
        private readonly List<string> _urlPrefixesToProxy;
        private readonly ILogger _logger;

        public OmegaProxyMiddleware(
            RequestDelegate next,
            OmegaServiceRegistration omegaServiceRegistration,
            IEnvironmentSettings envSettings,
            List<string> urlPrefixesToProxy)
        {
            _next = next;
            _omegaServiceRegistration = omegaServiceRegistration;
            _envSettings = envSettings;
            _urlPrefixesToProxy = urlPrefixesToProxy;
            _logger = Log.ForContext<OmegaProxyMiddleware>();
        }

        public async Task InvokeAsync(HttpContext context)
        {
            _logger.Debug("Running OmegaProxyMiddleware for URL: {Url}", context.Request.GetDisplayUrl());

            if (!context.RequestPathStartsWith("/api/"))
            {
                _logger.Debug("Does not start with /api/, calling next()");
                await _next(context);
            }
            else
            {
                if (ShouldProxyRequest(context))
                {
                    _logger.Debug("Using proxy for request: {Url}", context.Request.GetDisplayUrl());
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
            IEnvironmentSettings envSettings,
            List<string> urlPrefixesToProxy)
        {
            return builder.UseMiddleware<OmegaProxyMiddleware>(omegaServiceRegistration, envSettings, urlPrefixesToProxy);
        }
    }
}
