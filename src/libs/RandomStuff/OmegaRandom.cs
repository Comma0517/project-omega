using System;
using System.Linq;

namespace RandomStuff
{
    public class OmegaRandom
    {
        private static Random random = new Random();
        
        public static string getRandomString(int length)
        {
            if (length < 1)
            {
                throw new ArgumentOutOfRangeException("length", "length must be greater than 0");
            }
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz01234567890123456789";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }
        
        public static int getRandomNumber(int lower, int upper)
        {
            if (lower == upper)
            {
                return lower;
            }
            if (lower > upper)
            {
                throw new ArgumentException("upper must be greater than lower");
            }
            return random.Next(lower, upper);
        }
    }
}
