using System;
using Microsoft.Extensions.Logging;
using RandomStuff;

namespace Weather
{
    public class FakeWeatherGetter
    {
        public FakeWeatherGetter()
        {
        }
        
        public string GetRandomTempMessage()
        {
            return $"It is {OmegaRandom.getRandomNumber(0, 120)} degrees fahrenheit in some alternate dimension";
        }
    }
}
