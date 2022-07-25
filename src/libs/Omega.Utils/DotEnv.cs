using System;
using System.IO;

namespace Omega.Utils
{
    public static class DotEnv
    {
        public static void Load(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return;
            }

            Console.WriteLine("DotEnv is loading environment variables from " + filePath);

            foreach (var line in File.ReadAllLines(filePath))
            {
                var parts = line.Split('=', StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length != 2)
                    continue;

                Environment.SetEnvironmentVariable(parts[0], parts[1]);
            }
        }
        
        public static void Load()
        {
            Load(Path.Combine(Directory.GetCurrentDirectory(), ".env"));
        }
    }
}
