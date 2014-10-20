using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Carnotaurus.GhostPubsMvc.Common.Extensions;
using Carnotaurus.GhostPubsMvc.Common.Helpers;
using Carnotaurus.GhostPubsMvc.Data.Interfaces;
using Carnotaurus.GhostPubsMvc.Data.Models.Entities;
using Carnotaurus.GhostPubsMvc.Data.Models.ViewModels;
using Carnotaurus.GhostPubsMvc.Managers.Interfaces;

namespace Carnotaurus.GhostPubsMvc.Managers.Implementation
{
    public class QueryManager : IQueryManager
    {
        private readonly IReadStore _reader;

        public QueryManager(IReadStore reader)
        {
            _reader = reader;
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

            var document = XDocument.Load(requestUri);

            var element = document.Element("GeocodeResponse");

            return element;
        }

        public List<XElement> ReadElements(Org org)
        {
            var elements = new List<XElement>();


            var nso = ReadNsoResponseElement(org);
            elements.Add(nso);

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

            var document = XDocument.Load(requestUri);

            var element = document.Element("result");

            return element;
        }

        public IEnumerable<Org> GetOrgsToUpdate()
        {
            var results = _reader.Items<Org>().Where(org =>
                org != null
                && org.Address != null
                && org.Postcode != null
                && org.AddressTypeId == 1
                && org.CountyId == null
                && (org.Tried == null || org.TriedLa == null)
                && org.HauntedStatus == 1
                )
                .Take(100)
                .ToList();

            return results;
        }

        public County GetCounty(string name)
        {
            var results = _reader.Items<County>()
                .FirstOrDefault(x => x.Name == name);

            return results;
        }

        public IEnumerable<Region> GetRegions()
        {
            var results = _reader.Items<Region>();

            return results;
        }

        public IEnumerable<County> GetHauntedCountiesInRegion(Int32 regionId)
        {
            var regions = _reader.Items<Region>();

            var region = regions.First(r => r.Id == regionId);

            var results =
                region.Counties.Where(county => county.Orgs.Any(org => org.HauntedStatus == 1));

            return results;
        }


        public List<PageLinkModel> GetSitemapData(string currentRoot)
        {
            var data = _reader.Items<Org>();

            var queryable = data
                .Where(org => org.HauntedStatus == 1)
                .ToList()
                .GroupBy(org => org.UncRelTownPath)
                .Select(x => new KeyValuePair<String, Int32>(x.Key, x.Count()))
                .ToList();

            var ranked = queryable.RankByDescending(i => i.Value,
                (i, r) => new { Rank = r, Item = i })
                .ToList();

            var index = 1;

            var results = ranked.Select(pair =>
                CreatePageLinkModel(currentRoot, pair.Item, ref index, pair.Rank)
                )
                .ToList();

            // Key and Group
            return results;
        }


        private PageLinkModel CreatePageLinkModel(string currentRoot, KeyValuePair<string, int> pathKeyValuePair,
            ref int index, int rank)
        {
            if (pathKeyValuePair.Key.IsNullOrEmpty())
            {
                throw new Exception(
                    "The key is null or empty; this is usually because the CountryID or AddressTypeID is null or [HauntedOrgs_Fix] has not been run.");
            }

            var townLineage = new TownModel(pathKeyValuePair.Key);

            var data = _reader.Items<Org>();

            var queryable = data
                .Where(org => org.HauntedStatus == 1
                              && org.County.Region.Name == townLineage.Region
                              && org.County.Name == townLineage.County
                              && org.Town == townLineage.Town)
                .ToList()
                .Select(x => x.ExtractFullLink())
                .ToList();

            var result = new PageLinkModel(currentRoot)
            {
                Text = string.Format("{0}. {1}", rank, townLineage.FriendlyDescription),
                Title =
                    string.Format("{0} ({1} pubs in this area)",
                        townLineage.FriendlyDescription, pathKeyValuePair.Value),
                Unc = string.Format("{0}\\{1}", currentRoot, pathKeyValuePair.Key),
                Id = index - 1,
                Links = queryable
            };

            return result;
        }
         
        public void WriteWebmasterSitemap(List<string> sitepmap, String currentRoot)
        {

            // generate the Google webmaster tools xml url sitemap
            var sb = new StringBuilder();

            sb.AppendLine(@"<?xml version=""1.0"" encoding=""UTF-8""?>");

            sb.AppendLine(
                @"<urlset xmlns=""http://www.sitemaps.org/schemas/sitemap/0.9"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:schemaLocation=""http://www.sitemaps.org/schemas/sitemap/0.9 http://www.sitemaps.org/schemas/sitemap/0.9/sitemap.xsd"">");

            if (sitepmap != null)
            {
                foreach (var item in sitepmap)
                {
                    sb.AppendLine(item);
                }
            }

            sb.AppendLine("</urlset>");

            var fullFilePath = String.Format("{0}/ghostpubs-sitemap.xml", currentRoot);

            FileSystemHelper.WriteFile(fullFilePath, sb.ToString());

        }


        public String BuildPath(params String[] builder)
        {
            var output = String.Empty;

            foreach (var s in builder)
            {
                output = output == String.Empty ?
                    s.ToLower().SeoFormat() :
                    string.Format("{0}\\{1}", output, s.ToLower().SeoFormat());
            }

            return output;
        }
        

    }
}