using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Omega.Plumbing.Http;

namespace OmegaInterop.Core
{
    public class CoreClient : OmegaHttpClient, ICoreClient
    {
        public CoreClient(HttpClient httpClient) : base(httpClient)
        {
        }

        public async Task<string> GetCoreDummyMessage()
        {
            return await _httpClient.GetStringAsync("api/Core/DummyCore");
        }

        public async Task<List<OmegaUser>> GetOmegaUsers()
        {
            return await _httpClient.GetFromJsonAsync<List<OmegaUser>>($"api/Core/OmegaUsers");
        }
    }
}
