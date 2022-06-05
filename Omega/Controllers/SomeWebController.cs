using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace Omega.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SomeWebController : ControllerBase
    {
        private readonly ILogger<SomeWebController> _logger;
        private static Random random = new Random();

        public SomeWebController(ILogger<SomeWebController> logger)
        {
            _logger = logger;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetStuff()
        {
            _logger.LogInformation("SomeWebController default route called - will call into core service");
            string responseString;
            var handler = new HttpClientHandler
            {
                ClientCertificateOptions = ClientCertificateOption.Manual,
                ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => true
            };
            using (var client = new HttpClient(handler))
            {
                responseString = await client.GetStringAsync($"http://{EnvHelper.CORE_HOST}:{EnvHelper.CORE_PORT}/api/Core");
            }
            
            return new JsonResult(new {fromSomeWeb = "stuff from web controller", fromCore = responseString});
        }
    }
}
