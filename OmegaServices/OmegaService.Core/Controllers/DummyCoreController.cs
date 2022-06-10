using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using RandomStuff;

namespace OmegaService.Core.Controllers
{
    [ApiController]
    [Route("api/Core/[controller]")]
    public class DummyCoreController : ControllerBase
    {
        private readonly ILogger<DummyCoreController> _logger;
        private static Random random = new Random();

        public DummyCoreController(ILogger<DummyCoreController> logger)
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
