using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Carnotaurus.GhostPubsMvc.Common.Bespoke;
using Carnotaurus.GhostPubsMvc.Common.Bespoke.Enumerations;
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


        public StringResult ExtractCountyName(XContainer result)
        {
            var xmlResult = new StringResult();

            var countyResult =
                result.Elements("address_component")
                    .FirstOrDefault(x => x.Value.EndsWith("administrative_area_level_2political"));

            if (countyResult == null || countyResult.FirstNode == null) return xmlResult;

            var inner = countyResult.FirstNode as XElement;

            if (inner == null) return xmlResult;

            xmlResult.Result = inner.Value;

            return xmlResult;
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

            XDocument document = null;

            try
            {
                document = XDocument.Load(requestUri);
            }
            catch (Exception ex)
            {
                // throw new Exception(ex.InnerException.Message);
            }

            if (document != null)
            {
                var element = document.Element("GeocodeResponse");

                return element;
            }

            return null;
        }

        public List<XElement> ReadElements(Org org)
        {
            var elements = new List<XElement>();

            if (org.Postcode.Length >= 6)
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
            /*
             This XML file does not appear to have any style information associated with it. The document tree is shown below.
<result>
<postcode>SK1 3JT</postcode>
<geo>
<lat>53.40086635686018</lat>
<lng>-2.1493842139459183</lng>
<easting>390164.0</easting>
<northing>389348.0</northing>
<geohash>http://geohash.org/gcqrz16fedyz</geohash>
</geo>
<administrative>
<council>
<title>Stockport</title>
<uri>
http://statistics.data.gov.uk/id/statistical-geography/E08000007
</uri>
<code>E08000007</code>
</council>
<ward>
<title>Manor</title>
<uri>
http://statistics.data.gov.uk/id/statistical-geography/E05000793
</uri>
<code>E05000793</code>
</ward>
<constituency>
<title>Stockport</title>
<uri>
http://statistics.data.gov.uk/id/statistical-geography/E14000969
</uri>
<code>E14000969</code>
</constituency>
</administrative>
</result>
             */

            // http://uk-postcodes.com/postcode/SK13JT.xml

            var requestUri =
                string.Format(
                    "http://uk-postcodes.com/postcode/{0}.xml",
                    org.Postcode.Replace(" ", "")
                    );

            XDocument document = null;

            try
            {
                document = XDocument.Load(requestUri);
            }
            catch (Exception ex)
            {
                // throw new Exception(ex.InnerException.Message);
            }

            if (document != null)
            {
                var element = document.Element("result");

                return element;
            }

            return null;
        }
    }
}