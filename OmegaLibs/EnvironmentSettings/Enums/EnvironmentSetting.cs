using EnvironmentSettings.Attributes;

namespace EnvironmentSettings.Enums
{
    public enum EnvironmentSetting
    {
        [SettingInfo(defaultValue: "some_env default value")]
        SOME_ENV_VAR,
        [SettingInfo(defaultValue: "another_var has this as the default value")]
        ANOTHER_VAR,
        A_VAR_WITH_NO_ATTR
    }
}
