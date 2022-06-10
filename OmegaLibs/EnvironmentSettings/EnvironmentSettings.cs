using System.Security.AccessControl;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Omega.Utils;

namespace EnvironmentSettings
{
    public class EnvironmentSettings
    {
        private readonly ILogger<EnvironmentSettings> _logger;
        private readonly Dictionary<string, string> environmentVariables = new Dictionary<string, string>();
        
        public EnvironmentSettings(ILogger<EnvironmentSettings> logger)
        {
            _logger = logger;
        }

        public void AddSettings<TEnum>() where TEnum : System.Enum
        {
            var values = Enum.GetValues(typeof(TEnum));

            foreach (Enum val in values)
            {
                _logger.LogInformation("Name: " + val.ToName());
                var info = val.GetAttribute<SettingInfo>() ?? new SettingInfo();
                _logger.LogInformation("Default value: " + info.DefaultValue);
            }
        }

        private SettingInfo GetSettingInfo(Enum e)
        {
            
            return null;
        }
    }
}
