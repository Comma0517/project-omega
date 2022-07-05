using EnvironmentSettings.Logic;

namespace Omega.Plumbing.Data
{
    public abstract class BaseRepositoryTest
    {
        protected EnvSettings EnvSettings { get; set; }
        
        protected BaseRepositoryTest()
        {
            EnvSettings = new EnvSettings(new EnvironmentVariableProvider());
            EnvSettings.AddSettings<GlobalSettings>();
        }
    }
}
