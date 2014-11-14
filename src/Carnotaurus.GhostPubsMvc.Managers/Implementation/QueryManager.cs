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

        public OutputViewModel PrepareTownModel(string currentCountyPath,
            IEnumerable<KeyValuePair<string, PageLinkModel>> pubTownLinks, string town,
            string currentCountyName, Authority currentRegion, string currentRegionPath, string currentCountyDescription,
            int currentCountyId,
            string currentRoot, List<OutputViewModel> history)
        {
            var townPath = BuildPath(currentCountyPath, town);

            var pubLinks = pubTownLinks
                .Where(x => x.Key.Equals(town))
                .Select(x => x.Value)
                .ToList();

            var townModel = OutputViewModel.CreateTownOutputViewModel(currentCountyPath, town, currentCountyName,
                currentRegion,
                currentRegionPath, currentCountyDescription,
                currentCountyId, pubLinks, townPath, currentRoot, history);

            return townModel;
        }

        public OutputViewModel PreparePageTypeModel(PageTypeEnum pageType, string priority, string description,
            List<PageLinkModel> links,
            string title, string path, string currentRoot)
        {
            if (title == null)
            {
                title = pageType.ToString().CamelCaseToWords();
            }

            var model = OutputViewModel.CreatePageTypeOutputViewModel(pageType, priority, description, links, title,
                path, currentRoot);

            return model;
        }

        public OutputViewModel PrepareRegionModel(Authority currentRegion, string currentRegionPath,
            int orgsInRegionCount,
            IEnumerable<Authority> hauntedCountiesInRegion, string currentRoot,
            List<OutputViewModel> history)
        {
            var countyLinks = hauntedCountiesInRegion.Select(x => new PageLinkModel(currentRoot)
            {
                Text = x.Name,
                Title = x.Name
            }).ToList();

            var regionModel = OutputViewModel.CreateRegionOutputViewModel(currentRegion, currentRegionPath,
                orgsInRegionCount, countyLinks, currentRoot, history);

            return regionModel;
        }

        public OutputViewModel PrepareCountyModel(string currentCountyName, int currentCountyId,
            string currentCountyPath,
            IEnumerable<string> towns, Authority currentRegion, int count, string currentRegionPath, string currentRoot,
            List<OutputViewModel> history)
        {
            var townLinks = towns.Select(s => new PageLinkModel(currentRoot)
            {
                Text = s,
                Title = s
            }).ToList();

            var countyModel = OutputViewModel.CreateCountyOutputViewModel(currentCountyName, currentCountyId,
                currentCountyPath, currentRegion, count,
                currentRegionPath, townLinks, currentRoot, history);
            return countyModel;
        }

        public OutputViewModel PreparePubModel(ICollection<KeyValuePair<string, PageLinkModel>> pubTownLinks,
            string currentTownPath, Org pub, string currentRoot, List<OutputViewModel> history
            )
        {
            pub.TownPath = currentTownPath;

            pubTownLinks.Add(new KeyValuePair<string, PageLinkModel>(pub.Town, pub.ExtractLink(currentRoot)));

            var notes = pub.Notes.Select(note => new PageLinkModel(currentRoot)
            {
                Id = note.Id,
                Text = note.Text,
                Title = note.Text
            }).ToList();

            const PageTypeEnum action = PageTypeEnum.Pub;

            var pubModel = OutputViewModel.CreatePubOutputViewModel(pub, action, notes, currentRoot, history);

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

        public IEnumerable<Authority> GetHauntedCountiesInRegion(Int32 regionId)
        {
            var regions = _reader.Items<Authority>().ToList();

            var filtered = regions.Where(x =>
                x.ParentId == regionId
                && x.ParentAuthority.IsRegion
                && x.HasHauntedOrgs);

            return filtered;
        }


        public List<PageLinkModel> GetSitemapData(string currentRoot)
        {
            var data = _reader.Items<Org>();

            var queryable = data
                .Where(org => org.HauntedStatus == true)
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


        public string PrepareWebmasterSitemap(List<string> sitepmap)
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

            return sb.ToString();
        }


        public String BuildPath(params String[] builder)
        {
            var output = String.Empty;

            foreach (var s in builder)
            {
                output = output == String.Empty
                    ? s.ToLower().SeoFormat()
                    : string.Format("{0}\\{1}", output, s.ToLower().SeoFormat());
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

            var townLineage = new TownModel(pathKeyValuePair.Key);

            var data = _reader.Items<Org>();

            var queryable = data
                .Where(org => org.HauntedStatus == true
                              && org.Authority.ParentAuthority.Name == townLineage.Region
                              && org.Authority.Name == townLineage.County
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
    }
}