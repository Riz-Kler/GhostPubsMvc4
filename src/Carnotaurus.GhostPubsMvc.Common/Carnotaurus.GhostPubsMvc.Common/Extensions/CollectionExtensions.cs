using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Carnotaurus.GhostPubsMvc.Common.Extensions
{
    public static class CollectionExtensions
    {
        public static string ToCommaSeparatedString(this IEnumerable<string> inputList)
        {
            var output = String.Join(",", inputList.ToArray());
            return output;
        }

        public static Int32 ToInt32(this NameValueCollection collection, String key)
        {
            var result = collection[key].ToInt32();

            return result;
        }

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
    }
}