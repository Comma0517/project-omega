using System;

namespace Omega
{
    static class EnvHelper
    {
        public static bool IS_WEB;
        public static string CORE_HOST;
        public static string CORE_PORT;
        
        public static void Init()
        {
            IS_WEB = GetBool("IS_WEB");
            CORE_HOST = GetString("CORE_HOST");
            CORE_PORT = GetString("CORE_PORT");
        }
        
        public static string GetString(string name, string @default = null)
        {
            return Environment.GetEnvironmentVariable(name) ?? @default;
        }
        
        public static bool GetBool(string name, bool @default = false)
        {
            try
            {
                var envVar = Environment.GetEnvironmentVariable(name);
                return bool.Parse(envVar);
            }
            catch
            {
                return @default;
            }
        }
    }
}