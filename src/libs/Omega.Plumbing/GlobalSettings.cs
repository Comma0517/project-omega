using MikeyT.EnvironmentSettingsNS.Attributes;
using MikeyT.EnvironmentSettingsNS.Enums;

namespace Omega.Plumbing
{
    public enum GlobalSettings
    {
        [SettingInfo(ShouldLogValue = true)] ASPNETCORE_ENVIRONMENT,

        [SettingInfo(ShouldLogValue = true)] SERVICE_KEY,

        [SettingInfo(DefaultValue = "localhost", DefaultForEnvironment = DefaultSettingForEnvironment.LocalOnly, ShouldLogValue = true)]
        OMEGA_DEFAULT_DB_HOST,

        [SettingInfo(DefaultValue = "1434", DefaultForEnvironment = DefaultSettingForEnvironment.LocalOnly, ShouldLogValue = true)]
        OMEGA_DEFAULT_DB_PORT,

        [SettingInfo(DefaultValue = "sa", DefaultForEnvironment = DefaultSettingForEnvironment.LocalOnly)]
        OMEGA_DEFAULT_DB_USER,

        [SettingInfo(DefaultValue = "Abc1234!", DefaultForEnvironment = DefaultSettingForEnvironment.LocalOnly)]
        OMEGA_DEFAULT_DB_PASS,

        [SettingInfo(DefaultValue = "localhost", ShouldLogValue = true, DefaultForEnvironment = DefaultSettingForEnvironment.LocalOnly)]
        CORE_HOST,

        [SettingInfo(DefaultValue = "5001", ShouldLogValue = true, DefaultForEnvironment = DefaultSettingForEnvironment.LocalOnly)]
        CORE_PORT,

        [SettingInfo(DefaultValue = "localhost", ShouldLogValue = true, DefaultForEnvironment = DefaultSettingForEnvironment.LocalOnly)]
        WEATHER_HOST,

        [SettingInfo(DefaultValue = "5001", ShouldLogValue = true, DefaultForEnvironment = DefaultSettingForEnvironment.LocalOnly)]
        WEATHER_PORT
    }
}
