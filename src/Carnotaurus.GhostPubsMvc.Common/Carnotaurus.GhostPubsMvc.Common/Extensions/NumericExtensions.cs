using System;

namespace Carnotaurus.GhostPubsMvc.Common.Extensions
{
    public static class NumericExtensions
    {
        public static Int32 ToInt32(this Int32? nullable)
        {
            var result = 0;

            if (nullable.HasValue)
            {
                result = nullable.Value;
            }

            return result;
        }

        public static Boolean IsAboveZero(this Int32 value)
        {
            var result = value > 0;

            return result;
        }

        public static Int32 ToInt32(this Double value)
        {
            var result = Convert.ToInt32(value);

            return result;
        }


        public static Int64 ToInt64(this Double value)
        {
            var result = Convert.ToInt64(value);

            return result;
        }
    }
}