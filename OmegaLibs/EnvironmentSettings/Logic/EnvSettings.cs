using System;
using System.Collections.Generic;
using System.Text;
using EnvironmentSettings.Attributes;
using EnvironmentSettings.Enums;
using EnvironmentSettings.Interface;
using EnvironmentSettings.Model;
using Omega.Utils;

namespace EnvironmentSettings.Logic
{
    public class EnvSettings : IEnvSettings
    {
        public const string IS_LOCAL_KEY = "IS_LOCAL";

        private readonly IEnvironmentVariableProvider _environmentVariableProvider;
        private readonly Dictionary<string, EnvSettingWrapper> _envSettingWrappers = new();

        public EnvSettings(IEnvironmentVariableProvider environmentVariableProvider)
        {
            _environmentVariableProvider = environmentVariableProvider;
        }

        public void AddSettings<TEnum>() where TEnum : Enum
        {
            var enumValues = Enum.GetValues(typeof(TEnum));

            bool isLocal = IsLocal();

            foreach (Enum enumVal in enumValues)
            {
                var enumName = enumVal.ToName();
                var info = enumVal.GetAttribute<SettingInfo>() ?? new SettingInfo();

                var envVarValue = _environmentVariableProvider.GetEnvironmentVariable(enumName);
                
                string settingValue;
                if (info.DefaultForEnvironment == DefaultSettingForEnvironment.LocalOnly && !isLocal && envVarValue == null)
                {
                    settingValue = null;
                }
                else
                {
                    settingValue = envVarValue ?? info.DefaultValue;                    
                }
                
                var envSettingWrapper = new EnvSettingWrapper {Key = enumName, Value = settingValue, SettingInfo = info};
                
                _envSettingWrappers.Add(enumName, envSettingWrapper);
            }
        }

        public string GetString(Enum e)
        {
            return GetString(e.ToName());
        }

        public string GetString(Enum e, string defaultValue)
        {
            return GetString(e.ToName(), defaultValue);
        }

        public int GetInt(Enum e)
        {
            return GetInt(e.ToName());
        }

        public int? GetInt(Enum e, int? defaultValue)
        {
            return GetInt(e.ToName(), defaultValue);
        }

        public bool GetBool(Enum e)
        {
            return GetBool(e.ToName());
        }

        public bool GetBool(Enum e, bool defaultValue)
        {
            return GetBool(e.ToName(), defaultValue);
        }

        public string GetString(string name)
        {
            var value = _envSettingWrappers.ContainsKey(name) ? _envSettingWrappers[name].Value : null;

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ApplicationException($"Setting not loaded: " + name);
            }

            return value;
        }

        public string GetString(string name, string defaultValue)
        {
            if (!_envSettingWrappers.ContainsKey(name))
            {
                return defaultValue;
            }

            var value = _envSettingWrappers[name].Value;
            return string.IsNullOrWhiteSpace(value) ? defaultValue : value;
        }

        public int GetInt(string name)
        {
            var value = _envSettingWrappers.ContainsKey(name) ? _envSettingWrappers[name].Value : null;

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ApplicationException($"Setting not loaded: " + name);
            }

            try
            {
                return int.Parse(value);
            }
            catch (Exception)
            {
                throw new ApplicationException("Could not parse setting to int: " + name);
            }
        }

        public int? GetInt(string name, int? defaultValue)
        {
            if (!_envSettingWrappers.ContainsKey(name))
            {
                return defaultValue;
            }

            var stringValue = _envSettingWrappers[name].Value;
            if (string.IsNullOrWhiteSpace(stringValue))
            {
                return defaultValue;
            }

            try
            {
                return int.Parse(stringValue);
            }
            catch (Exception)
            {
                throw new ApplicationException("Could not parse setting to int: " + name);
            }
        }

        public bool GetBool(string name)
        {
            var value = _envSettingWrappers.ContainsKey(name) ? _envSettingWrappers[name].Value : null;

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ApplicationException($"Setting not loaded: " + name);
            }

            try
            {
                return bool.Parse(value);
            }
            catch (Exception)
            {
                throw new ApplicationException("Could not parse setting to bool: " + name);
            }
        }

        public bool GetBool(string name, bool defaultValue)
        {
            if (!_envSettingWrappers.ContainsKey(name))
            {
                return defaultValue;
            }

            var stringValue = _envSettingWrappers[name].Value;
            if (string.IsNullOrWhiteSpace(stringValue))
            {
                return defaultValue;
            }

            try
            {
                return bool.Parse(stringValue);
            }
            catch (Exception)
            {
                throw new ApplicationException("Could not parse setting to bool: " + name);
            }
        }

        public string GetAllAsSafeLogString()
        {
            var sb = new StringBuilder();
            sb.Append("Environment settings loaded:\n");
            foreach (var (key, envSettingWrapper) in _envSettingWrappers)
            {
                string value;
                if (envSettingWrapper.SettingInfo.SettingType == SettingType.Secret)
                {
                    value = "secret";
                    if (string.IsNullOrWhiteSpace(envSettingWrapper.Value))
                    {
                        value += " - empty";
                    }
                }
                else
                {
                    value = envSettingWrapper.SettingInfo.ShouldLogValue ? envSettingWrapper.Value : "not whitelisted";
                    if (!envSettingWrapper.SettingInfo.ShouldLogValue && string.IsNullOrWhiteSpace(envSettingWrapper.Value))
                    {
                        value += " - empty";
                    }
                }

                sb.Append($"{key}={value}\n");
            }

            return sb.ToString();
        }

        public bool IsLocal()
        {
            try
            {
                var stringVal = _environmentVariableProvider.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                return !string.IsNullOrWhiteSpace(stringVal) && stringVal.Trim() == "Development";
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
