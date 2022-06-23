using EnvironmentSettings.Attributes;

namespace EnvironmentSettings.Model
{
    public class EnvSettingWrapper
    {
        public string Key { get; init; }
        public string Value { get; init; }
        public SettingInfo SettingInfo { get; init; }
    }
}