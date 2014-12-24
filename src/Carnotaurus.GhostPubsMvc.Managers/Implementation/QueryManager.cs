using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Carnotaurus.GhostPubsMvc.Common.Bespoke.Enumerations;
using Carnotaurus.GhostPubsMvc.Common.Extensions;
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

        public OutputViewModel PrepareLocalityModel(
            IEnumerable<KeyValuePair<string, PageLinkModel>> orgLocalityLinks, string locality,
            Authority authority,
            string currentRoot)
        {
            if (orgLocalityLinks == null) throw new ArgumentNullException("orgLocalityLinks");
            if (locality == null) throw new ArgumentNullException("locality");
            if (authority == null) throw new ArgumentNullException("authority");

            var links = orgLocalityLinks
                .Where(x => x.Key.Equals(locality))
                .Select(x => x.Value)
                .ToList();

            // todo - dpc - come back - history
            var next = new PageLinkModel(currentRoot);

            var model = OutputViewModel.CreateLocalityOutputViewModel(locality, authority,
                links, currentRoot, next);

            return model;
        }

        public OutputViewModel PreparePageTypeModel(PageTypeEnum pageType, string priority, string description,
            List<PageLinkModel> links,
            string title, string currentRoot)
        {
            if (priority == null) throw new ArgumentNullException("priority");
            if (description == null) throw new ArgumentNullException("description");
            if (links == null) throw new ArgumentNullException("links");
            if (currentRoot == null) throw new ArgumentNullException("currentRoot");

            if (title == null)
            {
                title = pageType.ToString().CamelCaseToWords();
            }

            var model = OutputViewModel.CreatePageTypeOutputViewModel(pageType, priority, description, links, title,
                currentRoot);

            return model;
        }

        public OutputViewModel PrepareRegionModel(Authority region,
            int orgsInRegionCount,
            IEnumerable<Authority> hauntedAuthoritiesInRegion, string currentRoot)
        {
            if (region == null) throw new ArgumentNullException("region");
            if (hauntedAuthoritiesInRegion == null) throw new ArgumentNullException("hauntedAuthoritiesInRegion");
            if (currentRoot == null) throw new ArgumentNullException("currentRoot");

            var authorityLinks = hauntedAuthoritiesInRegion.Select(authority => new PageLinkModel(currentRoot)
            {
                Text = authority.Name,
                Title = authority.Name,
                Filename = authority.QualifiedNameDashified
            }).ToList();

            var next = region.GetNext();
             
            var model = OutputViewModel.CreateRegionOutputViewModel(region,
                orgsInRegionCount, authorityLinks, currentRoot, next);

            return model;
        }

        public OutputViewModel PrepareAuthorityModel(Authority authority, List<PageLinkModel> localities, int count,
            string currentRoot)
        {
            if (authority == null) throw new ArgumentNullException("authority");
            if (localities == null) throw new ArgumentNullException("localities");
            if (currentRoot == null) throw new ArgumentNullException("currentRoot");

            // todo - dpc - come back - history
            var next = new PageLinkModel(currentRoot);

            // dpc - cheshire-west-and-chester-ua.html should contain links to localities, such as: duddon-in-cheshire-west-and-chester-ua.html
            var model = OutputViewModel.CreateAuthorityOutputViewModel(authority, count,
                localities, currentRoot, next);

            return model;
        }

        public OutputViewModel PrepareOrgModel(
            Org org, string currentRoot)
        {
            if (org == null) throw new ArgumentNullException("org");

            var nex = org.GetNextOrg();

            var next = new PageLinkModel(currentRoot);

            // todo - dpc - come back - history
            if (nex != null)
            {
                next = new PageLinkModel(currentRoot)
                {
                    Title = nex.JumboTitle
                };

            }

            var model = OutputViewModel.CreateOrgOutputViewModel(org, currentRoot, next);

            return model;
        }

        public IEnumerable<Org> GetOrgsToUpdate()
        {
            var results = _reader.Items<Org>()
                .Where(org =>
                    org != null
                    && org.HauntedStatus.HasValue && org.HauntedStatus.Value
                    && org.AddressTypeId == 1
                    && org.Address != null
                    && org.Postcode != null
                    && org.Authority != null
                    && (!org.LaTried || !org.Tried)
                )
                //.Take(1)
                .ToList();

            return results;
        }

        public Authority GetAuthority(string code)
        {
            var results = _reader.Items<Authority>()
                .FirstOrDefault(x => x.Code == code);

            return results;
        }

        public IEnumerable<Authority> GetRegions()
        {
            var results = _reader.Items<Authority>()
                .ToList()
                .Where(x => x.IsRegion && x.HasHauntedOrgs)
                .OrderBy(o => o.Name);

            return results;
        }

        // this only seems to work for every authority except county and metropolian county (these have an extra tier of districts)
        public IEnumerable<Authority> GetHauntedFirstDescendantAuthoritiesInRegion(Int32 regionId)
        {
            var list = _reader.Items<Authority>().ToList();

            var inRegion = list
                .Where(x =>
                    x.ParentId == regionId
                    && x.ParentAuthority.IsRegion
                    && x.HasHauntedOrgs
                )
                .ToList();

            return inRegion;
        }


        public List<PageLinkModel> GetSitemapData(string currentRoot)
        {
            if (currentRoot == null) throw new ArgumentNullException("currentRoot");

            var data = _reader.Items<Org>();

            var queryable = data
                .Where(org => org.HauntedStatus == true)
                .ToList()
                .GroupBy(org => org.GeoPath)
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

        public string PrepareWebmasterSitemap(List<string> items)
        {
            // generate the Google webmaster tools xml url sitemap
            var sb = new StringBuilder();

            sb.AppendLine(@"<?xml version=""1.0"" encoding=""UTF-8""?>");

            sb.AppendLine(
                @"<urlset xmlns=""http://www.sitemaps.org/schemas/sitemap/0.9"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:schemaLocation=""http://www.sitemaps.org/schemas/sitemap/0.9 http://www.sitemaps.org/schemas/sitemap/0.9/sitemap.xsd"">");

            if (items != null)
            {
                foreach (var item in items)
                {
                    sb.AppendLine(item);
                }
            }

            sb.AppendLine("</urlset>");

            return sb.ToString();
        }

        private PageLinkModel CreatePageLinkModel(string currentRoot, KeyValuePair<string, int> pathKeyValuePair,
            ref int index, int rank)
        {
            if (currentRoot == null) throw new ArgumentNullException("currentRoot");

            if (pathKeyValuePair.Key.IsNullOrEmpty())
            {
                throw new Exception(
                    "The key is null or empty; this is usually because the CountryID or AddressTypeID is null or [HauntedOrgs_Fix] has not been run.");
            }

            var lineage = new LocalityModel(pathKeyValuePair.Key);

            var data = _reader.Items<Org>();

            var queryable = data
                .Where(org => org.HauntedStatus.HasValue
                              && org.HauntedStatus.Value
                              && org.Authority.ParentAuthority.Name == lineage.Region
                              && org.Authority.Name == lineage.Division
                              && org.Locality == lineage.Locality)
                .ToList()
                .Select(x => x.ExtractFullLink())
                .ToList();

            var result = new PageLinkModel(currentRoot)
            {
                Text = string.Format("{0}. {1}", rank, lineage.FriendlyDescription),
                Title =
                    string.Format("{0} ({1} pubs in this area)",
                        lineage.FriendlyDescription, pathKeyValuePair.Value),
                Filename = string.Format("{0}\\{1}", currentRoot, pathKeyValuePair.Key),
                Id = index - 1,
                Links = queryable
            };

            return result;
        }
    }
}