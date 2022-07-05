using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Omega.Plumbing.Proxy;

namespace Omega.Plumbing.Http
{
    public static class OmegaHttpContextExtensions
    {
        private const int StreamCopyBufferSize = 81920;

        public static bool RequestPathStartsWith(this HttpContext context, string startsWith)
        {
            return context.Request.Path.Value != null && context.Request.Path.Value.StartsWith(startsWith, StringComparison.OrdinalIgnoreCase);
        }

        public static async Task ProxyRequest(this HttpContext context, Uri destinationUri)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (destinationUri == null)
            {
                throw new ArgumentNullException(nameof(destinationUri));
            }

            var proxyService = context.RequestServices.GetRequiredService<ProxyService>();

            using var requestMessage = context.CreateProxyHttpRequest(destinationUri);
            var prepareRequestHandler = proxyService.Options.PrepareRequest;
            if (prepareRequestHandler != null)
            {
                await prepareRequestHandler(context.Request, requestMessage);
            }

            using var responseMessage = await context.SendProxyHttpRequest(requestMessage);
            await context.CopyProxyHttpResponse(responseMessage);
        }

        public static HttpRequestMessage CreateProxyHttpRequest(this HttpContext context, Uri uri)
        {
            var request = context.Request;

            var requestMessage = new HttpRequestMessage();
            var requestMethod = request.Method;
            if (!HttpMethods.IsGet(requestMethod) &&
                !HttpMethods.IsHead(requestMethod) &&
                !HttpMethods.IsDelete(requestMethod) &&
                !HttpMethods.IsTrace(requestMethod))
            {
                var streamContent = new StreamContent(request.Body);
                requestMessage.Content = streamContent;
            }

            // Copy the request headers
            foreach (var header in request.Headers)
            {
                if (!requestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray()) && requestMessage.Content != null)
                {
                    requestMessage.Content?.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
                }
            }

            requestMessage.Headers.Host = uri.Authority;
            requestMessage.RequestUri = uri;
            requestMessage.Method = new HttpMethod(request.Method);

            return requestMessage;
        }

        public static Task<HttpResponseMessage> SendProxyHttpRequest(this HttpContext context, HttpRequestMessage requestMessage)
        {
            if (requestMessage == null)
            {
                throw new ArgumentNullException(nameof(requestMessage));
            }

            var proxyService = context.RequestServices.GetRequiredService<ProxyService>();

            return proxyService.Client.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, context.RequestAborted);
        }

        public static async Task CopyProxyHttpResponse(this HttpContext context, HttpResponseMessage responseMessage)
        {
            if (responseMessage == null)
            {
                throw new ArgumentNullException(nameof(responseMessage));
            }

            var response = context.Response;

            response.StatusCode = (int) responseMessage.StatusCode;
            foreach (var header in responseMessage.Headers)
            {
                response.Headers[header.Key] = header.Value.ToArray();
            }

            foreach (var header in responseMessage.Content.Headers)
            {
                response.Headers[header.Key] = header.Value.ToArray();
            }

            // SendAsync removes chunking from the response. This removes the header so it doesn't expect a chunked response.
            response.Headers.Remove("transfer-encoding");

            await using var responseStream = await responseMessage.Content.ReadAsStreamAsync();
            await responseStream.CopyToAsync(response.Body, StreamCopyBufferSize, context.RequestAborted);
        }

        public static IServiceCollection AddProxy(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            return services.AddSingleton<ProxyService>();
        }

        public static IServiceCollection AddProxy(this IServiceCollection services, Action<SharedProxyOptions> configureOptions)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configureOptions == null)
            {
                throw new ArgumentNullException(nameof(configureOptions));
            }

            services.Configure(configureOptions);
            return services.AddSingleton<ProxyService>();
        }
    }
}
