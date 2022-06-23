using System.Collections;
using System.Collections.Generic;
using OmegaModel.Core;

namespace OmegaService.Core.Interface
{
    public interface IOmegaUserRepository
    {
        IEnumerable<OmegaUser> GetAllUsers();
    }
}
