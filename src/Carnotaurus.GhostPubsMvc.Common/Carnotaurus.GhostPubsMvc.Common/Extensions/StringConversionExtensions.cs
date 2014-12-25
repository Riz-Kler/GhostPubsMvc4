using System;

namespace Carnotaurus.GhostPubsMvc.Common.Extensions
{
    public static class StringConversionExtensions
    {
        public static Guid? ToNullableGuid(this String value)
        {
            Guid result;

            if (Guid.TryParse(value, out result))
            {
                return result;
            }

            return null;
        }

        public static Int32? ToNullableInt32(this String value)
        {
            int result;

            if (Int32.TryParse(value, out result))
            {
                return result;
            }

            return null;
        }

        public static double? ToNullableDouble(this String input)
        {
            double result;

            if (Double.TryParse(input, out result))
            {
                return result;
            }

            return null;
        }

        public static Decimal ToDecimal(this String value)
        {
            return value.ToNullableDecimal() ?? 0;
        }


        public static Int32 ToInt32(this String value)
        {
            var result = value.ToNullableInt32().ToInt32();

            return result;
        }

        public static Decimal? ToNullableDecimal(this String value)
        {
            Decimal result;

            if (Decimal.TryParse(value, out result))
            {
                return result;
            }

            return null;
        }
    }
}