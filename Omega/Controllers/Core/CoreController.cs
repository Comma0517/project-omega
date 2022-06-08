using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using RandomStuff;

namespace Omega.Controllers.Core
{
    [ApiController]
    [Route("api/[controller]")]
    public class CoreController : ControllerBase
    {
        private readonly ILogger<CoreController> _logger;
        private static Random random = new Random();

        public CoreController(ILogger<CoreController> logger)
        {
            _logger = logger;
        }
        
        [HttpGet]
        public string GetCoreMessage()
        {
            _logger.LogInformation("CoreController default route called");
            return "Hello from the core service. Here's a random number for you: " + OmegaRandom.getRandomNumber(1, 100);
        }
    }
}
