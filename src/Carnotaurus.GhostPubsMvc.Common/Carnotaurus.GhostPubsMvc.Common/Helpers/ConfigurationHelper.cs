using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Carnotaurus.GhostPubsMvc.Common.Extensions;

namespace Carnotaurus.GhostPubsMvc.Common.Helpers
{
    public class ConfigurationHelper
    {
        public static IDictionary<string, string> ToDictionary()
        {
            return ConfigurationManager.AppSettings.ToDictionary();
        }

        public static String GetValue(String key)
        {
            var result = GetNameValuePair(key);

            return result.Value;
        }

        public static KeyValuePair<string, string> GetNameValuePair(String key)
        {
            var result = ToDictionary().Where(x => x.Key == key);

            return result.First();
        }

        public static Int32 GetAsInt(String key)
        {
            return ConfigurationManager.AppSettings.ToInt32(key);
        }

        public static Boolean GetAsBoolean(String key)
        {
            return Convert.ToBoolean(GetAsInt(key));
        }

        public static String GetConnectionString(String name)
        {
            return ConfigurationManager.ConnectionStrings[name].ConnectionString;
        }
    }
}