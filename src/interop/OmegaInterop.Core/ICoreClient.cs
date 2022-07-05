using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace OmegaInterop.Core
{
    public interface ICoreClient
    {
        HttpClient GetHttpClient();
        Task<string> GetCoreDummyMessage();
        Task<List<OmegaUser>> GetOmegaUsers();
    }
}
