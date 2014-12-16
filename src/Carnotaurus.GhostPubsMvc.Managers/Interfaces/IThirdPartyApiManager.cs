using System.Collections.Generic;
using System.Xml.Linq;
using Carnotaurus.GhostPubsMvc.Common.Bespoke;
using Carnotaurus.GhostPubsMvc.Data.Models.Entities;

namespace Carnotaurus.GhostPubsMvc.Managers.Interfaces
{
    public interface IThirdPartyApiManager
    {
        List<XElement> ReadElements(Org org);

        XElement ReadGeocodeResponseElement(Org org);

        XElement ReadNsoResponseElement(Org org);

        XmlResult RequestGoogleMapsApiResponse(XElement element);

        XmlResult RequestLaApiResponse(XElement element);
    }
}