using System;
using System.Collections.Generic;
using System.Linq;
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
            IEnumerable<KeyValuePair<string, PageLinkModel>> pubTownLinks, string town,
            Authority currentCounty,
            string currentRoot, List<OutputViewModel> history)
        {
            var townPath = BuildPath(false, currentRoot, town);

            var pubLinks = pubTownLinks
                .Where(x => x.Key.Equals(town))
                .Select(x => x.Value)
                .ToList();

            var townModel = OutputViewModel.CreateLocalityOutputViewModel(town, currentCounty,
                pubLinks, townPath, currentRoot, history);

            return townModel;
        }

        public OutputViewModel PreparePageTypeModel(PageTypeEnum pageType, string priority, string description,
            List<PageLinkModel> links,
            string title, string currentRoot)
        {
            if (title == null)
            {
                title = pageType.ToString().CamelCaseToWords();
            }

            var model = OutputViewModel.CreatePageTypeOutputViewModel(pageType, priority, description, links, title,
                currentRoot);

            return model;
        }

        public OutputViewModel PrepareRegionModel(Authority currentRegion,
            int orgsInRegionCount,
            IEnumerable<Authority> hauntedCountiesInRegion, string currentRoot,
            List<OutputViewModel> history)
        {
            var countyLinks = hauntedCountiesInRegion.Select(x => new PageLinkModel(currentRoot)
            {
                Text = x.Name,
                Title = x.Name,
                Filename =
                    string.Format("{0}\\{1}\\{2}", currentRoot, x.RegionalLineage.ReverseItems().JoinWithBackslash(),
                        x.Name).Dashify()
            }).ToList();

            var regionModel = OutputViewModel.CreateRegionOutputViewModel(currentRegion,
                orgsInRegionCount, countyLinks, currentRoot, history);

            return regionModel;
        }

        public OutputViewModel PrepareAuthorityModel(Authority authority, IEnumerable<string> locations, int count,
            string currentRoot, List<OutputViewModel> history)
        {
            var links = locations.Select(s => new PageLinkModel(currentRoot)
            {
                Text = s,
                Title = s
            }).ToList();

            var model = OutputViewModel.CreateAuthorityOutputViewModel(authority, count,
                links, currentRoot, history);

            return model;
        }

        public OutputViewModel PreparePubModel(
            Org pub, string currentRoot, List<OutputViewModel> history
            )
        {
            var pubModel = OutputViewModel.CreatePubOutputViewModel(pub, currentRoot, history);

            return pubModel;
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
                .Where(x => x.IsRegion);

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


        //public List<PageLinkModel> GetSitemapData(string currentRoot)
        //{
        //    var data = _reader.Items<Org>();

        //    var queryable = data
        //        .Where(org => org.HauntedStatus == true)
        //        .ToList()
        //        .GroupBy(org => org.FilenameRelTownPath)
        //        .Select(x => new KeyValuePair<String, Int32>(x.Key, x.Count()))
        //        .ToList();

        //    var ranked = queryable.RankByDescending(i => i.Value,
        //        (i, r) => new { Rank = r, Item = i })
        //        .ToList();

        //    var index = 1;

        //    var results = ranked.Select(pair =>
        //        CreatePageLinkModel(currentRoot, pair.Item, ref index, pair.Rank)
        //        )
        //        .ToList();

        //    // Key and Group
        //    return results;
        //}


        //public string PrepareWebmasterSitemap(List<string> sitepmap)
        //{
        //    // generate the Google webmaster tools xml url sitemap
        //    var sb = new StringBuilder();

        //    sb.AppendLine(@"<?xml version=""1.0"" encoding=""UTF-8""?>");

        //    sb.AppendLine(
        //        @"<urlset xmlns=""http://www.sitemaps.org/schemas/sitemap/0.9"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:schemaLocation=""http://www.sitemaps.org/schemas/sitemap/0.9 http://www.sitemaps.org/schemas/sitemap/0.9/sitemap.xsd"">");

        //    if (sitepmap != null)
        //    {
        //        foreach (var item in sitepmap)
        //        {
        //            sb.AppendLine(item);
        //        }
        //    }

        //    sb.AppendLine("</urlset>");

        //    return sb.ToString();
        //}


        public String BuildPath(Boolean isLevelled, params String[] builder)
        {
            var output = String.Empty;

            var pattern = isLevelled ? "{0}\\{1}" : "{0}-{1}";

            foreach (var s in builder)
            {
                output = output == String.Empty
                    ? s.ToLower().Dashify()
                    : string.Format(pattern, output, s.ToLower().Dashify());
            }

            return output;
        }

        private PageLinkModel CreatePageLinkModel(string currentRoot, KeyValuePair<string, int> pathKeyValuePair,
            ref int index, int rank)
        {
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
                              && org.Authority.Name == lineage.County
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