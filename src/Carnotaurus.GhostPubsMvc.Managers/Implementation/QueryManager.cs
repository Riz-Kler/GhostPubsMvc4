﻿using System;
using System.Collections.Generic;
using System.Linq;
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


        public XElement ReadXElement(Org missingInfoOrg)
        {
            // source correct address, using google maps api or similar

            // E.G., https://maps.googleapis.com/maps/api/geocode/xml?address=26%20Smithfield,%20London,%20Greater%20London,%20EC1A%209LB,%20uk&sensor=true&key=AIzaSyC2DCdkPGBtsooyft7sX3P9h2f4uQvLQj0

            var key = ConfigurationHelper.GetValueAsString("GoogleMapsApiKey");
            // "AIzaSyC2DCdkPGBtsooyft7sX3P9h2f4uQvLQj0";

            var format =
                string.Format(
                    "https://maps.google.com/maps/api/geocode/xml?address={0}, {1}, {2}, UK&sensor=false&key={3}",
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
            var results = _reader.Items<Org>().Where(org =>
                org != null
                && org.Address != null
                && org.Postcode != null
                && org.AddressTypeId == 1
                && org.CountyId == null
                && org.Tried == null
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

            var region = regions.First(r => r.Id == regionId);

            var results =
                region.Counties.Where(county => county.Orgs.Any(org => org.HauntedStatus == 1));

            return results;
        }

        public List<PageLinkModel> GetSitemapData(string currentRoot)
        {
            // todo - do redirect sitemap and generation too?

            var data = _reader.Items<Org>();

            var queryable = data
                .Where(org => org.HauntedStatus == 1)
                .ToList()
                .GroupBy(org => org.UncRelTownPath)
                .Select(x => new KeyValuePair<String, Int32>(x.Key, x.Count()))
                .OrderByDescending(x => x.Value)
                .ToList();

            var index = 1;

            var results = queryable.Select(x =>
                CreatePageLinkModel(currentRoot, x, ref index)
                )
                .ToList();

            // todo - dpc - find out what this code was supposed to do
            foreach (var result in results)
            {
                var r = data.FirstOrDefault(q => q.Id == result.Id);
            }

            // Key and Group
            return results;
        }

        private static PageLinkModel CreatePageLinkModel(string currentRoot, KeyValuePair<string, int> pathKeyValuePair,
            ref int index)
        {
            if (pathKeyValuePair.Key.IsNullOrEmpty())
            {
                throw new Exception(
                    "The key is null or empty; this is usually because the CountryID or AddressTypeID is null or [HauntedOrgs_Fix] has not been run.");
            }

            var result = new PageLinkModel(currentRoot)
            {
                Text = string.Format("{0}. {1}", index++, pathKeyValuePair.Key.SplitOnSlash().JoinWithCommaReserve()),
                Title =
                    string.Format("{0} ({1} pubs in this area)",
                        pathKeyValuePair.Key.SplitOnSlash().JoinWithCommaReserve(), pathKeyValuePair.Value),
                Unc = string.Format("{0}\\{1}", currentRoot, pathKeyValuePair.Key),
                Id = index - 1,
                // todo - come back - card #185 - need to attach the orgs 
                // Links = data. ToList().Where(q => q.UncRelTownPath == x.Key),
                //.Select(r => new PageLinkModel(currentRoot)
                //    {
                //        Id = r.Id,
                //        Text = r.TradingName,
                //        Title = r.TradingName,
                //        Unc = r.ExtractLink(currentRoot).Unc,
                //        Links = null
                //    }).ToList() 
            };

            return result;
        }
    }
}