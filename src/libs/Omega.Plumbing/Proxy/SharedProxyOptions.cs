using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Omega.Plumbing.Proxy
{
    public class SharedProxyOptions
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public HttpMessageHandler MessageHandler { get; set; }

        public Func<HttpRequest, HttpRequestMessage, Task> PrepareRequest { get; set; }
    }
}
