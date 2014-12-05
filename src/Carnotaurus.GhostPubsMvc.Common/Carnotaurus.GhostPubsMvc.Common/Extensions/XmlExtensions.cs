using System;
using System.Xml.Linq;

namespace Carnotaurus.GhostPubsMvc.Common.Extensions
{
    public static class XmlExtensions
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
    }
}