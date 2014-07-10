using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Carnotaurus.GhostPubsMvc.Common.Extensions
{
    public static class PrimativeExtensions
    {
        public static IDictionary<string, string> ToDictionary(this NameValueCollection col)
        {
            IDictionary<string, string> dict = new Dictionary<string, string>();

            foreach (var k in col.AllKeys)
            {
                dict.Add(k, col[k]);
            }

            return dict;
        }

        public static string Join(this IEnumerable<string> collection, String delimiter)
        {
            return String.Join(delimiter, collection);
        }

        public static string JoinWithComma(this IEnumerable<string> collection)
        {
            return collection.Join(", ");
        }

        public static string OxbridgeAnd(this IEnumerable<string> collection)
        {
            var output = String.Empty;

            var list = collection.ToList();

            if (list.Count > 1)
            {
                var delimited = String.Join(", ", list.Take(list.Count - 1));

                output = String.Concat(delimited, ", and ", list.LastOrDefault());
            }

            return output;
        }


        public static Boolean IsNotNullOrEmpty(this String value)
        {
            var isNullOrEmpty = !value.IsNullOrEmpty();

            return isNullOrEmpty;
        }

        public static Boolean IsNullOrEmpty(this String value)
        {
            var isNullOrEmpty = String.IsNullOrEmpty(value);

            return isNullOrEmpty;
        }

        public static Boolean IsNotEmpty(this Guid value)
        {
            return !value.IsEmpty();
        }

        public static Boolean IsEmpty(this Guid value)
        {
            return value == Guid.Empty;
        }

        public static Boolean IsNullOrEmpty(this Guid? value)
        {
            return !value.HasValue || value.Value.IsEmpty();
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

        public static Int32 ToInt32(this Int32? nullable)
        {
            var result = 0;

            if (nullable.HasValue)
            {
                result = nullable.Value;
            }

            return result;
        }

        public static Int32 ToInt32(this NameValueCollection collection, String key)
        {
            var result = collection[key].ToInt32();

            return result;
        }

        public static Boolean IsAboveZero(this Int32 value)
        {
            var result = value > 0;

            return result;
        }
    }
}