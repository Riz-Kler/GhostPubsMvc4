using System;
using System.Collections.Generic;
using System.Linq;
using Carnotaurus.GhostPubsMvc.Common.Bespoke.Enumerations;
using Carnotaurus.GhostPubsMvc.Common.Extensions;
using Carnotaurus.GhostPubsMvc.Data.Interfaces;
using Carnotaurus.GhostPubsMvc.Data.Models.Entities;

namespace Carnotaurus.GhostPubsMvc.Data.Models.ViewModels
{
    [Serializable]
    public class OutputViewModel : IOutputViewModel
    {
        #region Statics

        public static OutputViewModel CreateAllUkRegionsOutputViewModel(string currentRoot, List<Authority> regions)
        {
            if (regions == null) throw new ArgumentNullException("regions");

            var viewModel = new OutputViewModel(currentRoot)
            {
                JumboTitle = "Haunted pubs in UK by region",
                Action = PageTypeEnum.Country,
                PageLinks = regions.Select(x => x.Name != null
                    ? new PageLinkModel(currentRoot)
                    {
                        Text = x.Name,
                        Title = x.Name,
                        Unc = string.Format(@"\{0}", x.Name.SeoFormat())
                    }
                    : null).OrderBy(x => x.Text).ToList(),
                MetaDescription = string.Format("Haunted pubs in {0}",
                    regions.Select(region => region.Name).OxbridgeAnd()),
                ArticleDescription = string.Format("Haunted pubs in {0}",
                    regions.Select(region => region.Name).OxbridgeAnd()),
                Unc = currentRoot,
                Parent = new KeyValuePair<string, string>("Home page", @"/"),
                Priority = PageTypePriority.Country,
            };

            return viewModel;
        }

        public static OutputViewModel CreateRegionOutputViewModel(Authority currentRegion, string currentRegionPath,
            int orgsInRegionCount, IList<PageLinkModel> countyLinks, String currentRoot,
            IEnumerable<OutputViewModel> history)
        {
            var regionModel = new OutputViewModel(currentRoot)
            {
                JumboTitle = currentRegion.Name,
                Action = PageTypeEnum.Region,
                PageLinks = countyLinks.Select(x => x.Text != null
                    ? new PageLinkModel(currentRoot)
                    {
                        Text = x.Text,
                        Title = x.Text,
                        Unc =
                            string.Format(@"\{0}\{1}", currentRegion.Name.SeoFormat(),
                                x.Text.SeoFormat())
                    }
                    : null).OrderBy(x => x.Text).ToList(),
                MetaDescription = string.Format("Haunted pubs in {0}", countyLinks.Select(x => x.Text).OxbridgeAnd()),
                ArticleDescription = string.Format("Haunted pubs in {0}", countyLinks.Select(x => x.Text).OxbridgeAnd()),
                Unc = currentRegionPath,
                Parent =
                    new KeyValuePair<string, string>(currentRegion.Name,
                        currentRegion.Name.SeoFormat().ToLower()),
                Total = orgsInRegionCount,
                Priority = PageTypePriority.Region,
                Previous = history.LastOrDefault(x => x.Action == PageTypeEnum.Region),
                Lineage = new Breadcrumb
                {
                    Region = new PageLinkModel(currentRoot)
                    {
                        Unc = currentRegionPath,
                        Id = currentRegion.Id,
                        Text = currentRegion.Name,
                        Title = currentRegion.Name,
                    },
                }
            };
            return regionModel;
        }

        public static OutputViewModel CreateCountyOutputViewModel(string currentCountyName, int currentCountyId,
            string currentCountyPath, Authority currentRegion, int count, string currentRegionPath,
            IList<PageLinkModel> townLinks,
            String currentRoot,
            IEnumerable<OutputViewModel> history)
        {
            var countyModel = new OutputViewModel(currentRoot)
            {
                JumboTitle = currentCountyName,
                Action = PageTypeEnum.County,
                PageLinks = townLinks.Select(linkModel => linkModel.Text != null
                    ? new PageLinkModel(currentRoot)
                    {
                        Text = linkModel.Text,
                        Title = linkModel.Text,
                        Unc =
                            string.Format(@"\{0}\{1}\{2}",
                                currentRegion.Name.SeoFormat(),
                                currentCountyName.SeoFormat(),
                                linkModel.Text.SeoFormat()
                                )
                    }
                    : null).OrderBy(x => x.Text).ToList(),
                MetaDescription = string.Format(
                    "Haunted pubs in {0}",
                    townLinks.Select(x => x.Text).OxbridgeAnd()),
                ArticleDescription = string.Format(
                    "Haunted pubs in {0}",
                    townLinks.Select(x => x.Text).OxbridgeAnd()),
                Unc = currentCountyPath,
                Parent = new KeyValuePair<string, string>(currentRegion.Name,
                    currentRegion.Name.SeoFormat().ToLower()),
                Total = count,
                Priority = PageTypePriority.County,
                Previous = history.LastOrDefault(x => x.Action == PageTypeEnum.County),
                Lineage = new Breadcrumb
                {
                    Region = new PageLinkModel(currentRoot)
                    {
                        Unc = currentRegionPath,
                        Id = currentRegion.Id,
                        Text = currentRegion.Name,
                        Title = currentRegion.Name
                    },
                    County = new PageLinkModel(currentRoot)
                    {
                        Unc = currentCountyPath,
                        Id = currentCountyId,
                        Text = currentCountyName,
                        Title = currentCountyName
                    }
                }
            };
            return countyModel;
        }

        public static OutputViewModel CreateTownOutputViewModel(string currentCountyPath, string town,
            string currentCountyName,
            Authority currentRegion, string currentRegionPath, string currentCountyDescription, int currentCountyId,
            IList<PageLinkModel> pubLinks,
            string townPath, string currentRoot, IEnumerable<OutputViewModel> history)
        {
            var townModel = new OutputViewModel(currentRoot)
            {
                JumboTitle = town,
                Action = PageTypeEnum.Town,
                PageLinks = pubLinks.Select(x => x.Text != null
                    ? new PageLinkModel(currentRoot)
                    {
                        Text = x.Text,
                        Title = x.Title,
                        Unc =
                            string.Format(@"\{0}\{1}\{2}\{3}\{4}", currentRegion.Name.SeoFormat(),
                                currentCountyName.SeoFormat(),
                                town.SeoFormat(), x.Id, x.Text.SeoFormat())
                    }
                    : null).OrderBy(x => x.Text).ToList(),
                MetaDescription = string.Format("{0}, {1}, {2}", town, currentCountyName, currentRegion.Name),
                ArticleDescription = string.Format("{0}, {1}, {2}", town, currentCountyName, currentRegion.Name),
                Unc = townPath,
                Parent = new KeyValuePair<string, string>(currentCountyDescription, String.Empty),
                Total = pubLinks.Count(),
                Priority = PageTypePriority.Town,
                Previous = history.LastOrDefault(x => x.Action == PageTypeEnum.Town),
                Lineage = new Breadcrumb
                {
                    Region = new PageLinkModel(currentRoot)
                    {
                        Unc = currentRegionPath,
                        Id = currentRegion.Id,
                        Text = currentRegion.Name,
                        Title = currentRegion.Name
                    },
                    County = new PageLinkModel(currentRoot)
                    {
                        Unc = currentCountyPath,
                        Id = currentCountyId,
                        Text = currentCountyName,
                        Title = currentCountyName
                    },
                    Town = new PageLinkModel(currentRoot)
                    {
                        Unc = townPath,
                        Id = currentCountyId,
                        Text = town,
                        Title = town
                    }
                }
            };
            return townModel;
        }

        public static OutputViewModel CreatePubOutputViewModel(Org pub,
            PageTypeEnum action,
            IList<PageLinkModel> notes,
            String currentRoot,
            IEnumerable<OutputViewModel> history)
        {
            var pubModel = new OutputViewModel(currentRoot)
            {
                JumboTitle = pub.TradingName,
                Action = action,
                PageLinks = notes,
                MetaDescription =
                    string.Format("{0}, {1} : {2}", pub.Address, pub.PostcodePrimaryPart, notes.First().Text),
                ArticleDescription = string.Format("{0}, {1}", pub.Address, pub.PostcodePrimaryPart),
                Unc = pub.Path,
                Parent = new KeyValuePair<string, string>(pub.Town, pub.Town.SeoFormat().ToLower()),
                Tags = pub.Tags.Select(x => x.Feature.Name).ToList(),
                Priority = PageTypePriority.Pub,
                Previous = history.LastOrDefault(x => x.Action == action),
                Lat = pub.Lat.ToString(),
                Lon = pub.Lon.ToString(),
                OtherNames = pub.Authority.Orgs
                    .Where(x => x.Address == pub.Address && x.Postcode == pub.Postcode && x.Id != pub.Id)
                    .Select(
                        org => new PageLinkModel(currentRoot)
                        {
                            Id = org.Id,
                            Text = org.TradingName,
                            Title = org.TradingName,
                            Unc =
                                string.Format("{0}/{1}/{2}", pub.TownPath.SeoFormat(), org.Id,
                                    org.TradingName.SeoFormat())
                        }
                    ).ToList(),
                Lineage = new Breadcrumb
                {
                    Region = new PageLinkModel(currentRoot)
                    {
                        Unc = pub.RegionPath,
                        Id = pub.Id,
                        Text = pub.Authority.ParentAuthority.Name,
                        Title = pub.Authority.ParentAuthority.Name
                    },
                    County = new PageLinkModel(currentRoot)
                    {
                        Unc = pub.CountyPath,
                        Id = pub.Id,
                        Text = pub.Authority.Name,
                        Title = pub.Authority.Name
                    },
                    Town = new PageLinkModel(currentRoot)
                    {
                        Unc = pub.TownPath,
                        Id = pub.Id,
                        Text = pub.Town,
                        Title = pub.Town
                    },
                    Pub = new PageLinkModel(currentRoot)
                    {
                        Unc = pub.Path,
                        Id = pub.Id,
                        Text = pub.TradingName,
                        Title = pub.TradingName
                    }
                }
            };
            return pubModel;
        }

        public static OutputViewModel CreatePageTypeOutputViewModel(PageTypeEnum pageType, string priority,
            string description,
            List<PageLinkModel> links, string title, string path, string currentRoot)
        {
            var model = new OutputViewModel(currentRoot)
            {
                JumboTitle = title,
                Action = pageType,
                MetaDescription = description,
                ArticleDescription = description,
                Unc = path,
                Priority = priority,
                PageLinks = links,
                Total = links != null ? links.Count() : 0
            };
            return model;
        }

        #endregion Statics

        private readonly String _currentRoot = String.Empty;

        public OutputViewModel(String currentRoot)
        {
            PageLinks = new List<PageLinkModel>();
            Tags = new List<String>();
            _currentRoot = currentRoot;
        }

        public String Lat { get; set; }

        public String Lon { get; set; }

        public Breadcrumb Lineage { get; set; }

        public String MetaDescription { get; set; }

        public String ArticleDescription { get; set; }

        public String Link { get; set; }

        public String Tag { get; set; }

        public IList<PageLinkModel> OtherNames { get; set; }

        public IList<PageLinkModel> PageLinks { get; set; }

        public String Unc { get; set; }

        public String Url
        {
            get
            {
                var result = String.Concat(Unc, @"\", "detail.html").ToLower();

                if (!_currentRoot.IsNullOrEmpty())
                {
                    result = String.Format("http://www.ghostpubs.com/haunted-pubs{0}",
                        result.Replace(_currentRoot.ToLower(), String.Empty).Replace("\\", "/"));
                }

                return result;
            }
        }

        public KeyValuePair<String, String> Parent { get; set; }

        public IEnumerable<String> Tags { get; set; }

        public Int32 Total { get; set; }

        public String Priority { get; set; }

        public OutputViewModel Previous { get; set; }

        public String SitemapItem
        {
            get
            {
                //<url>
                //<loc>http://www.mypubguide.com/dne/app10/pages/pub/pub-95584.aspx</loc>
                //<lastmod>2012-01-12T22:06:02+00:00</lastmod>
                //<changefreq>daily</changefreq>
                //<priority>0.9</priority>
                //</url>

                const string pattern =
                    "<url><loc>{0}</loc><lastmod>{1}</lastmod><changefreq>{2}</changefreq><priority>{3}</priority></url>";

                var output = String.Format(pattern,
                    Url.ToLower(),
                    DateTime.UtcNow.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'+00:00'"),
                    "daily",
                    Priority
                    );

                return output;
            }
        }

        public String JumboTitle { get; set; }

        public PageTypeEnum Action { get; set; }
    }
}