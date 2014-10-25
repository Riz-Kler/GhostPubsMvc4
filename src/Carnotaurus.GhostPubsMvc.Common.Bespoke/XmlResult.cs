using System.Xml.Linq;
using Carnotaurus.GhostPubsMvc.Common.Bespoke.Enumerations;

namespace Carnotaurus.GhostPubsMvc.Common.Bespoke
{
    public class XmlResult
    {
        public XmlResult()
        {
            ResultType = ResultTypeEnum.Fail;

            Result = new XElement("empty", null);
        }

        public ResultTypeEnum ResultType { get; set; }

        public XElement Result { get; set; }
    }
}