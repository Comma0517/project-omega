using EnvironmentSettings.Attributes;
using EnvironmentSettings.Enums;

namespace EnvironmentSettings.Tests
{
    public enum TestEnvSettings
    {
        SettingWithNoAttribute,

        [SettingInfo(DefaultValue = "some string", ShouldLogValue = true)]
        SomeStringSetting,

        [SettingInfo(DefaultValue = "42", ShouldLogValue = true)]
        SomeIntSetting,

        [SettingInfo(DefaultValue = "true", ShouldLogValue = true)]
        SomeBoolSettingTrue,

        [SettingInfo(DefaultValue = "false", ShouldLogValue = true)]
        SomeBoolSettingFalse,

        [SettingInfo(ShouldLogValue = true)] SettingWithNoDefaultValue,

        [SettingInfo(DefaultValue = "test_secret_local_only_default",
            SettingType = SettingType.Secret,
            DefaultForEnvironment = DefaultSettingForEnvironment.LocalOnly)]
        SomeSecretSettingWithLocalDefaultOnly,

        [SettingInfo(DefaultValue = "test_secret_all_environment_default",
            SettingType = SettingType.Secret,
            DefaultForEnvironment = DefaultSettingForEnvironment.AllEnvironments)]
        SomeSecretSettingWithAllEnvironmentsDefault
    }
}
