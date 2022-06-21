using EnvironmentSettings.Logic;

namespace Omega.Plumbing
{
    public class ConnectionStringProvider
    {
        private readonly EnvSettings _envSettings;
        private const string ENV_KEY_DB_HOST = "DEFAULT_DB_HOST";
        private const string ENV_KEY_DB_PORT = "DEFAULT_DB_PORT";
        private const string ENV_KEY_DB_USER = "DEFAULT_DB_USER";
        private const string ENV_KEY_DB_PASS = "DEFAULT_DB_PASS";
        
        public ConnectionStringProvider(EnvSettings envSettings)
        {
            _envSettings = envSettings;
        }

        public string GetConnectionString(string dbName, string host, string port, string userId, string password)
        {
            return $"Server={host},{port};Database={dbName};User Id={userId};Password={password};";
        }

        public string GetConnectionString(string dbName)
        {
            var host = _envSettings.GetString(ENV_KEY_DB_HOST);
            var port = _envSettings.GetString(ENV_KEY_DB_PORT);
            var user = _envSettings.GetString(ENV_KEY_DB_USER);
            var pass = _envSettings.GetString(ENV_KEY_DB_PASS);
            return GetConnectionString(dbName, host, port, user, pass);
        }
    }
}
