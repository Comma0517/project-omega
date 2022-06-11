using EnvironmentSettings.Attributes;
using EnvironmentSettings.Enums;

namespace OmegaService.Web
{
    public enum WebEnvSettings
    {
        [SettingInfo(DefaultValue = "localhost", ShouldLogValue = true, DefaultForEnvironment = DefaultSettingForEnvironment.LocalOnly)]
        CORE_HOST,
        
        [SettingInfo(DefaultValue = "5000", ShouldLogValue = true, DefaultForEnvironment = DefaultSettingForEnvironment.LocalOnly)]
        CORE_PORT,
        
        [SettingInfo(DefaultValue = "localhost", ShouldLogValue = true, DefaultForEnvironment = DefaultSettingForEnvironment.LocalOnly)]
        WEATHER_HOST,
        
        [SettingInfo(DefaultValue = "5000", ShouldLogValue = true, DefaultForEnvironment = DefaultSettingForEnvironment.LocalOnly)]
        WEATHER_PORT
    }
}
