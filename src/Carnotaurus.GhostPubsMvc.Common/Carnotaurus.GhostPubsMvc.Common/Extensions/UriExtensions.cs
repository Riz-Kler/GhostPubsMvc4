using System;
using System.IO;
using System.Net;
using System.Xml.Linq;

namespace Carnotaurus.GhostPubsMvc.Common.Extensions
{
    public static class UriExtensions
    {
        public static XElement GetElement(this Uri uri, string name)
        {
            XDocument document = null;

            try
            {
                document = XDocument.Load(uri.AbsoluteUri);
            }
            catch (Exception ex)
            {
                // throw new Exception(ex.InnerException.Message);
            }

            if (document == null) return null;

            var element = document.Element(name);

            return element;
        }

        public static string HttpGet(this Uri uri)
        {
            var result = String.Empty;

            try
            {
                var httpWebRequest = WebRequest.Create(uri) as HttpWebRequest;

                var cookieContainer = new CookieContainer();

                if (httpWebRequest != null)
                {
                    httpWebRequest.CookieContainer = cookieContainer;

                    var response = httpWebRequest.GetResponse();

                    var sr = new StreamReader(response.GetResponseStream());

                    result = sr.ReadToEnd().Trim();
                }
            }
            catch (Exception ex)
            {
                var exception = ex;
                // throw new Exception(ex.InnerException.Message);
            }

            return result;
        }
  
    }
}