using System.Reflection;
using Omega.Plumbing;
using static Serilog.Log;

namespace OmegaService.Auth
{
    public class AuthService : ProjectOmegaService
    {
        public AuthService(Assembly assembly) : base(assembly, ForContext<AuthService>())
        {
        }
    }
}
