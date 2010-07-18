using System;

namespace Zippy.Chirp
{
    public static class Utilities
    {
        public static bool Is(this string a, string b)
        {
            return string.Equals(a, b, StringComparison.InvariantCultureIgnoreCase);
        }

        public static int ToInt(this string input, int defaultValue)
        {
            int result;
            if (int.TryParse(input, out result))
                return result;
            else return defaultValue;
        }
    }
}
