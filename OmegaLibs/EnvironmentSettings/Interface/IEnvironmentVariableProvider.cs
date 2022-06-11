namespace EnvironmentSettings.Interface
{
    public interface IEnvironmentVariableProvider
    {
        string GetEnvironmentVariable(string name);
    }
}
