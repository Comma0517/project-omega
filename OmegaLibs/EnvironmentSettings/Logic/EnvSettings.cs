using System.Text;
using System.Security.AccessControl;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Omega.Utils;
using EnvironmentSettings.Attributes;

namespace EnvironmentSettings.Logic
{
    public class EnvSettings
    {
        private readonly ILogger<EnvSettings> _logger;
        private readonly Dictionary<string, Tuple<string, SettingInfo>> _environmentSettings = new Dictionary<string, Tuple<string, SettingInfo>>();

        public EnvSettings(ILogger<EnvSettings> logger)
        {
            _logger = logger;
        }

        public void AddSettings<TEnum>() where TEnum : System.Enum
        {
            var enumValues = Enum.GetValues(typeof(TEnum));

            foreach (Enum enumVal in enumValues)
            {
                var enumName = enumVal.ToName();
                // _logger.LogInformation("Name: " + enumName);
                var info = enumVal.GetAttribute<SettingInfo>() ?? new SettingInfo();

                string settingValue = Environment.GetEnvironmentVariable(enumName) ?? info.DefaultValue;
                // _logger.LogInformation("Default value: " + info.DefaultValue);
                _environmentSettings.Add(enumName, Tuple.Create(settingValue, info));
            }
        }
        
        public void LogSettingsLoaded()
        {
            var sb = new StringBuilder();
            sb.Append("\n--------------------\nEnvironment settings loaded:\n");
            foreach (var kvp in _environmentSettings)
            {
                // if (kvp.)
                
                sb.Append("");
            }
            _logger.LogInformation(sb.ToString());
        }
    }
}
