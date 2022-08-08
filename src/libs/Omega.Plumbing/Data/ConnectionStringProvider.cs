using MikeyT.EnvironmentSettingsNS.Interface;

namespace Omega.Plumbing.Data
{
    public class ConnectionStringProvider : IConnectionStringProvider
    {
        private readonly IEnvironmentSettings _envSettings;

        public ConnectionStringProvider(IEnvironmentSettings envSettings)
        {
            _envSettings = envSettings;
        }

        public string GetConnectionString(string dbName, string host, string port, string userId, string password)
        {
            return $"Server={host},{port};Database={dbName};User Id={userId};Password={password};";
        }

        public string GetConnectionString(string dbName)
        {
            var host = _envSettings.GetString(GlobalSettings.OMEGA_DEFAULT_DB_HOST);
            var port = _envSettings.GetString(GlobalSettings.OMEGA_DEFAULT_DB_PORT);
            var user = _envSettings.GetString(GlobalSettings.OMEGA_DEFAULT_DB_USER);
            var pass = _envSettings.GetString(GlobalSettings.OMEGA_DEFAULT_DB_PASS);
            return GetConnectionString(dbName, host, port, user, pass);
        }
    }
}
