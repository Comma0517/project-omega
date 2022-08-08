
using MikeyT.EnvironmentSettingsNS.Interface;
using MikeyT.EnvironmentSettingsNS.Logic;

namespace Omega.Plumbing.Data
{
    public abstract class BaseRepositoryTest
    {
        protected IEnvironmentSettings EnvSettings { get; set; }
        
        protected BaseRepositoryTest()
        {
            EnvSettings = new EnvironmentSettings(new EnvironmentVariableProvider());
            EnvSettings.AddSettings<GlobalSettings>();
        }
    }
}
