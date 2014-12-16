using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Carnotaurus.GhostPubsMvc.Common.Bespoke;
using Carnotaurus.GhostPubsMvc.Common.Bespoke.Enumerations;
using Carnotaurus.GhostPubsMvc.Common.Extensions;
using Carnotaurus.GhostPubsMvc.Common.Helpers;
using Carnotaurus.GhostPubsMvc.Data.Interfaces;
using Carnotaurus.GhostPubsMvc.Data.Models.Entities;
using Carnotaurus.GhostPubsMvc.Managers.Interfaces;

namespace Carnotaurus.GhostPubsMvc.Managers.Implementation
{
    public class ThirdPartyThirdPartyApiManager : IThirdPartyApiManager
    {
        private readonly IReadStore _reader;

        public ThirdPartyThirdPartyApiManager(IReadStore reader)
        {
            _reader = reader;
        }

        public XmlResult RequestGoogleMapsApiResponse(XElement xElement)
        {
            var result = new XmlResult();

            if (xElement == null || xElement.Value.Contains("OVER_QUERY_LIMIT"))
            {
                return result;
            }

            if (xElement.Value.Contains("ZERO_RESULTS"))
            {
                result.ResultType = ResultTypeEnum.NoResults;

                return result;
            }

            if (xElement.Element("result") == null)
            {
                return result;
            }

            result.ResultType = ResultTypeEnum.Success;

            result.Result = xElement.Element("result");

            return result;
        }

        public XmlResult RequestLaApiResponse(XElement xElement)
        {
            var result = new XmlResult();

            if (xElement == null)
            {
                return result;
            }

            var countained = xElement.Element("administrative");

            if (countained == null) return result;

            result.ResultType = ResultTypeEnum.Success;

            result.Result = countained;

            return result;
        }


        public List<XElement> ReadElements(Org org)
        {
            var elements = new List<XElement>();

            if (org.Postcode.IsNotNullOrEmpty() && org.Postcode.Length >= 6)
            {
                var nso = ReadNsoResponseElement(org);
                if (nso != null)
                {
                    elements.Add(nso);
                }
            }

            var geocode = ReadGeocodeResponseElement(org);
            elements.Add(geocode);

            return elements;
        }

        public XElement ReadNsoResponseElement(Org org)
        {
            const string pattern = "http://uk-postcodes.com/postcode/{0}.xml";

            var requestUri =
                string.Format(
                    pattern,
                    org.Postcode.RemoveSpaces()
                    );

            var uri = new Uri(requestUri);

            return uri.GetElement("result");
        }


        public XElement ReadGeocodeResponseElement(Org org)
        {
            // source correct address, using google maps api or similar

            // E.G., https://maps.googleapis.com/maps/api/geocode/xml?address=26%20Smithfield,%20London,%20Greater%20London,%20EC1A%209LB,%20uk&sensor=true&key=AIzaSyC2DCdkPGBtsooyft7sX3P9h2f4uQvLQj0

            var key = ConfigurationHelper.GetValueAsString("GoogleMapsApiKey");
            // "AIzaSyC2DCdkPGBtsooyft7sX3P9h2f4uQvLQj0";

            var requestUri =
                string.Format(
                    "https://maps.google.com/maps/api/geocode/xml?address={0}, {1}, {2}, UK&sensor=false&key={3}",
                    org.TradingName,
                    org.Address,
                    org.Postcode,
                    key
                    );

            var uri = new Uri(requestUri);

            return uri.GetElement("GeocodeResponse");
        }
    }
}