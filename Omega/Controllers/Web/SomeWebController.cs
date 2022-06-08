using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace Omega.Controllers.Web
{
    [ApiController]
    [Route("api/[controller]")]
    public class SomeWebController : ControllerBase
    {
        private readonly ILogger<SomeWebController> _logger;
        private readonly string CORE_URL_BASE;
        private static Random random = new Random();
        private static HttpClient httpClient = new HttpClient(new HttpClientHandler
        {
            ClientCertificateOptions = ClientCertificateOption.Manual,
            ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => true
        });

        public SomeWebController(ILogger<SomeWebController> logger)
        {
            _logger = logger;
            CORE_URL_BASE = $"http://{EnvHelper.CORE_HOST}:{EnvHelper.CORE_PORT}/api/";
        }

        [HttpGet]
        public async Task<IActionResult> GetStuff()
        {
            _logger.LogInformation("SomeWebController default route called - will call into core service");
            string responseString = await httpClient.GetStringAsync($"{CORE_URL_BASE}Core");

            return new JsonResult(new { fromSomeWeb = "stuff from web controller", fromCore = responseString });
        }
    }
}
