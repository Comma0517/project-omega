using EnvironmentSettings.Attributes;
using EnvironmentSettings.Enums;

namespace Omega.Plumbing
{
    public enum GlobalSettings
    {
        [SettingInfo(DefaultValue = "localhost", DefaultForEnvironment = DefaultSettingForEnvironment.LocalOnly, ShouldLogValue = true)]
        DEFAULT_DB_HOST,
        [SettingInfo(DefaultValue = "1433", DefaultForEnvironment = DefaultSettingForEnvironment.LocalOnly, ShouldLogValue = true)]
        DEFAULT_DB_PORT,
        [SettingInfo(DefaultValue = "sa", DefaultForEnvironment = DefaultSettingForEnvironment.LocalOnly)]
        DEFAULT_DB_USER,
        [SettingInfo(DefaultValue = "Abc1234!", DefaultForEnvironment = DefaultSettingForEnvironment.LocalOnly)]
        DEFAULT_DB_PASS
    }
}
