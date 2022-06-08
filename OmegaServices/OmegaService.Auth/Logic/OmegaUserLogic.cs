using System.Collections.Generic;
using OmegaService.Auth.Interface;
using SharedModel.Auth;

namespace OmegaService.Auth.Logic
{
    public class OmegaUserLogic : IOmegaUserLogic
    {
        public List<OmegaUser> GetAllUsers()
        {
            // TODO: return some dummy users
            return new List<OmegaUser>();
        }
    }
}
