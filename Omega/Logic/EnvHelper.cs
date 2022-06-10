using System;

namespace Omega.Logic
{
    static class EnvHelper
    {
        public static string SERVICE_KEY;
        public static string CORE_HOST;
        public static string CORE_PORT;
        public static string WEATHER_HOST;
        public static string WEATHER_PORT;
        
        public static void Init()
        {
            SERVICE_KEY = GetString("SERVICE_KEY");
            CORE_HOST = GetString("CORE_HOST") ?? "localhost";
            CORE_PORT = GetString("CORE_PORT") ?? "5000";
            WEATHER_HOST = GetString("WEATHER_HOST") ?? "localhost";
            WEATHER_PORT = GetString("WEATHER_PORT") ?? "5000";
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