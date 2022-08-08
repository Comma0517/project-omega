using MikeyT.EnvironmentSettingsNS.Interface;

namespace Omega.Plumbing.Data
{
    public class TestConnectionStringProvider : IConnectionStringProvider
    {
        private readonly ConnectionStringProvider _connectionStringProvider;

        public TestConnectionStringProvider(IEnvironmentSettings envSettings)
        {
            _connectionStringProvider = new ConnectionStringProvider(envSettings);
        }

        public string GetConnectionString(string dbName, string host, string port, string userId, string password)
        {
            var connectionString = _connectionStringProvider.GetConnectionString(dbName, host, port, userId, password);
            return connectionString.Replace("Database=", "Database=Test");
        }

        public string GetConnectionString(string dbName)
        {
            var connectionString = _connectionStringProvider.GetConnectionString(dbName);
            return connectionString.Replace("Database=", "Database=Test");
        }
    }
}
