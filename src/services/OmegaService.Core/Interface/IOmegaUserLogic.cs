using System.Collections.Generic;
using OmegaModel.Core;

namespace OmegaService.Core.Interface
{
    public interface IOmegaUserLogic
    {
        List<OmegaUser> GetAllUsers();
    }
}
