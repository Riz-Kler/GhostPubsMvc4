using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Carnotaurus.GhostPubsMvc.Common.Extensions
{
    public static class NameValueCollectionExtensions
    {
 
        public static Int32 ToInt32(this NameValueCollection collection, String key)
        {
            var result = collection[key].ToInt32();

            return result;
        }

        public static IDictionary<string, string> ToDictionary(this NameValueCollection col)
        {
            IDictionary<string, string> results = new Dictionary<string, string>();

            foreach (var key in col.AllKeys)
            {
                results.Add(key, col[key]);
            }

            return results;
        }
    }
}
