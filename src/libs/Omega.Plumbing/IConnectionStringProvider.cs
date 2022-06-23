namespace Omega.Plumbing
{
    public interface IConnectionStringProvider
    {
        string GetConnectionString(string dbName, string host, string port, string userId, string password);
        string GetConnectionString(string dbName);
    }
}
