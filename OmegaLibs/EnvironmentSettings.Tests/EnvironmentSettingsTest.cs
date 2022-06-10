using System;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using EnvironmentSettings.Logic;
using EnvironmentSettings.Enums;

namespace EnvironmentSettings.Tests
{
    public class EnvironmentSettingsTest
    {
        [Fact]
        public void Ctor_ItDoesStuff_Wee()
        {
            using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var logger = loggerFactory.CreateLogger<EnvSettings>();
            EnvSettings envSettings = new EnvSettings(logger);
            envSettings.AddSettings<EnvironmentSetting>(); // TODO: replace with testing enum
        }
    }
}
