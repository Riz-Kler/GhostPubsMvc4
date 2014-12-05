using System;
using System.IO;
using System.Net;

namespace Carnotaurus.GhostPubsMvc.Common.Extensions
{
    public static class NetExtensions
    {
        public static string HttpGet(this Uri uri)
        {
            var text = String.Empty;
            try
            {
                var req = WebRequest.Create(uri) as HttpWebRequest;
                var cookieContainer = new CookieContainer();
                if (req != null)
                {
                    req.CookieContainer = cookieContainer;
                    var resp = req.GetResponse();
                    var sr = new StreamReader(resp.GetResponseStream());
                    text = sr.ReadToEnd().Trim();
                }
            }
            catch (Exception ex)
            {
                var exception = ex;
                // throw new Exception(ex.InnerException.Message);
            }

            return text;
        }
    }
}