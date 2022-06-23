using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OmegaModel.Core;
using OmegaService.Core.Interface;

namespace OmegaService.Core.Controllers
{
    [ApiController]
    [Route("api/Core/[controller]")]
    public class OmegaUserController : ControllerBase
    {
        private readonly ILogger<OmegaUserController> _logger;
        private readonly IOmegaUserRepository _omegaUserRepository;

        public OmegaUserController(ILogger<OmegaUserController> logger, IOmegaUserRepository omegaUserRepository)
        {
            _logger = logger;
            _omegaUserRepository = omegaUserRepository;
        }

        [HttpGet("All")]
        public IEnumerable<OmegaUser> GetCoreMessage()
        {
            return _omegaUserRepository.GetAllUsers();
        }
    }
}
