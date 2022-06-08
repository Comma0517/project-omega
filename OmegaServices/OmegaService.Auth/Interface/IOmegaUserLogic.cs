using System.Collections.Generic;
using SharedModel.Auth;

namespace OmegaService.Auth.Interface
{
    public interface IOmegaUserLogic
    {
        List<OmegaUser> GetAllUsers();
    }
}
