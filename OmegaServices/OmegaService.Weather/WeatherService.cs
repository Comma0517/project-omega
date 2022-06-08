using System;
using OmegaPlumbing;

namespace OmegaService.Weather
{
    public class WeatherService : ProjectOmegaService
    {
        public WeatherService()
        {
        }

        public override void InitService()
        {
            Console.WriteLine("You dawg, initializing Weather P.O.S.");
        }
    }
}
