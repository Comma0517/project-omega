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
    public class RandomController : ControllerBase
    {
        private readonly ILogger<RandomController> _logger;
        private static Random random = new Random();

        public RandomController(ILogger<RandomController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            _logger.LogInformation("RandomController default route called");
            return Ok(getRandomString(20));
        }
        
        [HttpGet("strings/{numStrings}")]
        public IEnumerable<string> GetStrings(int? numStrings)
        {
            
            int num = numStrings ?? 0;
            const int MAX_NUM = 20;
            string ERROR_MSG = $"Num strings must be between 0 and {MAX_NUM}";
            
            if (numStrings > 20 || numStrings < 0)
            {
                throw new ApplicationException(ERROR_MSG);
            }
            
            var strings = new List<String>();
            for (int i = 0; i < numStrings; i++)
            {
                strings.Add(getRandomString(20));
            }
            
            return strings;
        }

        private string getRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz01234567890123456789";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
