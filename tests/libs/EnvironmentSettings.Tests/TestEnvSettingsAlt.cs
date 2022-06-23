using EnvironmentSettings.Attributes;

namespace EnvironmentSettings.Tests
{
    public enum TestEnvSettingsAlt
    {
        [SettingInfo(DefaultValue = "foo", ShouldLogValue = true)]
        SomeStringSettingAlt,

        [SettingInfo(DefaultValue = "777", ShouldLogValue = true)]
        SomeIntSettingAlt,
    }
}
