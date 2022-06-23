using System;
using EnvironmentSettings.Interface;

namespace EnvironmentSettings.Logic
{
    public class EnvironmentVariableProvider : IEnvironmentVariableProvider
    {
        public string GetEnvironmentVariable(string name)
        {
            return Environment.GetEnvironmentVariable(name);
        }
    }
}
