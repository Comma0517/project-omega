using System;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace EnvironmentSettings.Tests
{
    public class EnvironmentSettingsTest
    {
        [Fact]
        public void Ctor_ItDoesStuff_Wee()
        {
            using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var logger = loggerFactory.CreateLogger<EnvironmentSettings>();
            EnvironmentSettings envSettings = new EnvironmentSettings(logger);
            envSettings.AddSettings<EnvironmentSetting>();
        }
    }
}
