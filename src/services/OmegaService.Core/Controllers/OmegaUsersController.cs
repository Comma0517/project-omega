using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OmegaInterop.Core;
using OmegaService.Core.Interface;

namespace OmegaService.Core.Controllers
{
    [ApiController]
    [Route("api/Core/[controller]")]
    public class OmegaUsersController : ControllerBase
    {
        private readonly ILogger<OmegaUsersController> _logger;
        private readonly IOmegaUserRepository _omegaUserRepository;

        public OmegaUsersController(ILogger<OmegaUsersController> logger, IOmegaUserRepository omegaUserRepository)
        {
            _logger = logger;
            _omegaUserRepository = omegaUserRepository;
        }

        [HttpGet]
        public IEnumerable<OmegaUser> GetCoreMessage()
        {
            return _omegaUserRepository.GetAllUsers();
        }
    }
}
