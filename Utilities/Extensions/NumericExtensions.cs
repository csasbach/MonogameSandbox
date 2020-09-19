using System;

namespace Utilities.Extensions
{
    public static class NumericExtensions
    {
        public static int ToInt(this float floatingPointNumber)
        {
            return (int)Math.Round(floatingPointNumber);
        }
    }
}
