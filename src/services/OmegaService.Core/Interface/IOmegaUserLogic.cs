using System.Collections.Generic;
using OmegaInterop.Core;

namespace OmegaService.Core.Interface
{
    public interface IOmegaUserLogic
    {
        List<OmegaUser> GetAllUsers();
    }
}
