using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RandomStuff;
using Serilog;

namespace OmegaService.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RandomController : ControllerBase
    {
        private readonly ILogger<RandomController> _logger;

        public RandomController(ILogger<RandomController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            _logger.LogInformation("RandomController default route called");
            return Ok(OmegaRandom.getRandomString(20));
        }

        [HttpGet("strings/{numStrings}")]
        public IEnumerable<string> GetStrings(int? numStrings)
        {
            var num = numStrings ?? 0;
            const int MAX_NUM = 20;
            var ERROR_MSG = $"Num strings must be between 0 and {MAX_NUM}";

            if (num > 20 || num < 0)
            {
                throw new ApplicationException(ERROR_MSG);
            }

            var strings = new List<string>();
            for (var i = 0; i < num; i++)
            {
                strings.Add(OmegaRandom.getRandomString(20));
            }

            return strings;
        }
    }
}
