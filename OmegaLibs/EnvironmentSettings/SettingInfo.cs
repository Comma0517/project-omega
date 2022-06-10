namespace EnvironmentSettings
{
    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class SettingInfo : System.Attribute
    {
        public string DefaultValue { get; init; }
        public SettingType SettingType { get; init; }
        public DefaultSettingForEnvironment DefaultForEnvironment { get; init; }
        public bool ShouldLogValue { get; init; }

        public SettingInfo(string defaultValue = "",
                           SettingType settingType = SettingType.ENVIRONMENT_VARIABLE,
                           DefaultSettingForEnvironment defaultForEnvironment = DefaultSettingForEnvironment.ALL_ENVIRONMENTS,
                           bool shouldLogValue = false)
        {
            DefaultValue = defaultValue;
            SettingType = settingType;
            DefaultForEnvironment = defaultForEnvironment;
            ShouldLogValue = shouldLogValue;
        }
    }
}
