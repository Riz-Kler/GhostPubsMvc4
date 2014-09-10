using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Carnotaurus.GhostPubsMvc.Common.Extensions;
using Carnotaurus.GhostPubsMvc.Common.Helpers;
using Carnotaurus.GhostPubsMvc.Data.Interfaces;
using Carnotaurus.GhostPubsMvc.Data.Models;
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


        public XElement ReadXElement(Org missingInfoOrg)
        {
            // source correct address, using google maps api or similar

            // E.G., https://maps.googleapis.com/maps/api/geocode/xml?address=26%20Smithfield,%20London,%20Greater%20London,%20EC1A%209LB,%20uk&sensor=true&key=AIzaSyC2DCdkPGBtsooyft7sX3P9h2f4uQvLQj0

            var key = ConfigurationHelper.GetValueAsString("GoogleMapsApiKey");
            // "AIzaSyC2DCdkPGBtsooyft7sX3P9h2f4uQvLQj0";

            var format = string.Format("https://maps.google.com/maps/api/geocode/xml?address={0}, {1}, {2}, UK&sensor=false&key={3}",
                missingInfoOrg.TradingName,
                missingInfoOrg.Address,
                missingInfoOrg.Postcode,
                key
            );

            var requestUri = (format);

            var xdoc = XDocument.Load(requestUri);

            var xElement = xdoc.Element("GeocodeResponse");

            return xElement;
        }

        public IEnumerable<Org> GetMissingInfoOrgsToUpdate()
        {
            var results = _reader.Items<Org>().Where(f =>
                f != null
                && f.Address != null
                && f.Postcode != null
                && f.AddressTypeId == 1
                && f.CountyId == null
                && f.Tried == null
                )
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

            var region = regions.First(x => x.Id == regionId);

            var results =
                region.Counties.Where(x => x.Orgs.Any(y => y.HauntedStatus == 1));

            return results;
        }

        public class LeaderTown
        {
            public String Town { get; set; }
            public County County { get; set; }

        }

        public List<LinkModel> GetSitemapData(string currentRoot)
        {
            var data = _reader.Items<Org>();

            var queryable = data
                .Where(x => x.HauntedStatus == 1)
                .ToList()
                .GroupBy(x => x.Crab)
                .Select(x => new KeyValuePair<String, Int32>(x.Key, x.Count())).ToList();

            var results = queryable.OrderByDescending(x => x.Value).ToList();

            var index = 1;
            var q = results.Select(x => new LinkModel(currentRoot)
               {
                   Text = string.Format("{0}. {1}", index++, x.Key.SplitOnSlash().JoinWithCommaReserve()),
                   Title = string.Format("{0} ({1} pubs in this area)", x.Key.SplitOnSlash().JoinWithCommaReserve(), x.Value),
                   Unc = currentRoot + @"\" + x.Key,
                   Id = index-1
               }).ToList();

            // Key and Group
            return q;
        }

    }
}