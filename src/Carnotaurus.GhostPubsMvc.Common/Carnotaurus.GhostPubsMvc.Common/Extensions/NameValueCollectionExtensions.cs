using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Carnotaurus.GhostPubsMvc.Common.Extensions
{
    public static class NameValueCollectionExtensions
    {
        public static Int32 ToInt32(this NameValueCollection nameValueCollection, String key)
        {
            var result = nameValueCollection[key].ToInt32();

            return result;
        }

        public static IDictionary<string, string> ToDictionary(this NameValueCollection nameValueCollection)
        {
            IDictionary<string, string> results = new Dictionary<string, string>();

            foreach (var key in nameValueCollection.AllKeys)
            {
                results.Add(key, nameValueCollection[key]);
            }

            return results;
        }
    }
}