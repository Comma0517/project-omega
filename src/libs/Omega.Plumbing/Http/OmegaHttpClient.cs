using System.Net.Http;

namespace Omega.Plumbing.Http
{
    public class OmegaHttpClient
    {
        protected readonly HttpClient _httpClient;

        protected OmegaHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public HttpClient GetHttpClient()
        {
            return _httpClient;
        }
    }
}
