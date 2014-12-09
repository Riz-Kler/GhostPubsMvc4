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
                Filename = "UK",
                JumboTitle = "Haunted pubs in UK by region",
                Action = PageTypeEnum.Country,
                PageLinks = regions.Select(x => x.Name != null
                    ? new PageLinkModel(currentRoot)
                    {
                        Text = x.Name,
                        Title = x.Name,
                        Filename = string.Format(@"\{0}", x.Name.SeoFormat())
                    }
                    : null).OrderBy(x => x.Text).ToList(),
                MetaDescription = string.Format("Haunted pubs in {0}",
                    regions.Select(region => region.Name).OxbridgeAnd()),
                ArticleDescription = string.Format("Haunted pubs in {0}",
                    regions.Select(region => region.Name).OxbridgeAnd()),
                Parent = new KeyValuePair<string, string>("Home page", @"/"),
                Priority = PageTypePriority.Country,
            };

            return viewModel;
        }

        public static OutputViewModel CreateRegionOutputViewModel(Authority currentRegion,
            int orgsInRegionCount, IList<PageLinkModel> countyLinks, String currentRoot,
            IEnumerable<OutputViewModel> history)
        {
            var regionModel = new OutputViewModel(currentRoot)
            {
                Filename = currentRegion.Name,
                JumboTitle = currentRegion.Name,
                Action = PageTypeEnum.Region,
                PageLinks = countyLinks.Select(x => x.Text != null
                    ? new PageLinkModel(currentRoot)
                    {
                        Text = x.Text,
                        Title = x.Text,
                        Filename =
                            string.Format(@"\{0}\{1}", currentRegion.Name.SeoFormat(),
                                x.Text.SeoFormat())
                    }
                    : null).OrderBy(x => x.Text).ToList(),
                MetaDescription = string.Format("Haunted pubs in {0}", countyLinks.Select(x => x.Text).OxbridgeAnd()),
                ArticleDescription = string.Format("Haunted pubs in {0}", countyLinks.Select(x => x.Text).OxbridgeAnd()),
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
                        Filename = currentRegion.Name,
                        Id = currentRegion.Id,
                        Text = currentRegion.Name,
                        Title = currentRegion.Name,
                    },
                }
            };
            return regionModel;
        }

        public static OutputViewModel CreateCountyOutputViewModel(Authority authority, int count,
            IList<PageLinkModel> townLinks,
            String currentRoot,
            IEnumerable<OutputViewModel> history)
        {
            var countyModel = new OutputViewModel(currentRoot)
            {
                Filename = authority.Name,
                JumboTitle = authority.Name,
                Action = PageTypeEnum.County,
                PageLinks = townLinks.Select(linkModel => linkModel.Text != null
                    ? new PageLinkModel(currentRoot)
                    {
                        Text = linkModel.Text,
                        Title = linkModel.Text,
                        Filename =
                            string.Format(@"\{0}\{1}\{2}",
                                authority.ParentAuthority.Name.SeoFormat(),
                                authority.Name.SeoFormat(),
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
                Parent = new KeyValuePair<string, string>(authority.ParentAuthority.Name,
                    authority.ParentAuthority.Name.SeoFormat().ToLower()),
                Total = count,
                Priority = PageTypePriority.County,
                Previous = history.LastOrDefault(x => x.Action == PageTypeEnum.County),
                Lineage = new Breadcrumb
                {
                    Region = new PageLinkModel(currentRoot)
                    {
                        Filename = authority.ParentAuthority.Name,
                        Id = authority.ParentAuthority.Id,
                        Text = authority.ParentAuthority.Name,
                        Title = authority.ParentAuthority.Name
                    },
                    County = new PageLinkModel(currentRoot)
                    {
                        Filename = authority.Name,
                        Id = authority.Id,
                        Text = authority.Name,
                        Title = authority.Name
                    }
                }
            };
            return countyModel;
        }

        public static OutputViewModel CreateTownOutputViewModel(string town,

            Authority currentCounty, 
            IList<PageLinkModel> pubLinks,
            string townPath, string currentRoot, IEnumerable<OutputViewModel> history)
        {
            var townModel = new OutputViewModel(currentRoot)
            {
                Filename = (town + "-" + currentCounty.Name).SeoFormat(),
                JumboTitle = town + " in " + currentCounty.Name,
                Action = PageTypeEnum.Town,
                PageLinks = pubLinks.Select(x => x.Text != null
                    ? new PageLinkModel(currentRoot)
                    {
                        Text = x.Text,
                        Title = x.Title,
                        Filename =
                            string.Format(@"\{0}\{1}\{2}\{3}\{4}", currentCounty.ParentAuthority.Name.SeoFormat(),
                                currentCounty.Name.SeoFormat(),
                                town.SeoFormat(), x.Id, x.Text.SeoFormat())
                    }
                    : null).OrderBy(x => x.Text).ToList(),
                MetaDescription = string.Format("{0}, {1}, {2}", town, currentCounty.Name, currentCounty.ParentAuthority.Name),
                ArticleDescription = string.Format("{0}, {1}, {2}", town, currentCounty.Name, currentCounty.ParentAuthority.Name),
                Parent = new KeyValuePair<string, string>(currentCounty.ParentAuthority.Name, String.Empty),
                Total = pubLinks.Count(),
                Priority = PageTypePriority.Town,
                Previous = history.LastOrDefault(x => x.Action == PageTypeEnum.Town),
                Lineage = new Breadcrumb
                {
                    Region = new PageLinkModel(currentRoot)
                    {
                        Filename = currentCounty.ParentAuthority.Name,
                        Id = currentCounty.ParentAuthority.Id,
                        Text = currentCounty.ParentAuthority.Name,
                        Title = currentCounty.ParentAuthority.Name
                    },
                    County = new PageLinkModel(currentRoot)
                    {
                        Filename = currentCounty.Name,
                        Id = currentCounty.Id,
                        Text = currentCounty.Name,
                        Title = currentCounty.Name
                    },
                    Town = new PageLinkModel(currentRoot)
                    {
                        Filename = townPath,
                        Id = currentCounty.Id,
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
                Filename = pub.Id + "-" + pub.TradingName.SeoFormat() + "-" + pub.Locality.SeoFormat(),
                JumboTitle = pub.TradingName,
                Action = action,
                PageLinks = notes,
                MetaDescription =
                    string.Format("{0}, {1} : {2}", pub.Address, pub.PostcodePrimaryPart, notes.First().Text),
                ArticleDescription = string.Format("{0}, {1}", pub.Address, pub.PostcodePrimaryPart),
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
                            Filename =
                                string.Format("{0}/{1}/{2}", pub.TownPath.SeoFormat(), org.Id,
                                    org.TradingName.SeoFormat())
                        }
                    ).ToList(),
                Lineage = new Breadcrumb
                {
                    Region = new PageLinkModel(currentRoot)
                    {
                        Filename = pub.RegionPath,
                        Id = pub.Id,
                        Text = pub.Authority.ParentAuthority.Name,
                        Title = pub.Authority.ParentAuthority.Name
                    },
                    County = new PageLinkModel(currentRoot)
                    {
                        Filename = pub.CountyPath,
                        Id = pub.Id,
                        Text = pub.Authority.Name,
                        Title = pub.Authority.Name
                    },
                    Town = new PageLinkModel(currentRoot)
                    {
                        Filename = pub.TownPath,
                        Id = pub.Id,
                        Text = pub.Town,
                        Title = pub.Town
                    },
                    Pub = new PageLinkModel(currentRoot)
                    {
                        Filename = pub.Path,
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
                Filename = path,
                Priority = priority,
                PageLinks = links,
                Total = links != null ? links.Count() : 0
            };
            return model;
        }

        #endregion Statics

        public String CurrentRoot { get; set; }

        public OutputViewModel(String currentRoot)
        {
            PageLinks = new List<PageLinkModel>();
            Tags = new List<String>();
            CurrentRoot = currentRoot;
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

        public String Filename { get; set; }

        public String FriendlyFilename
        {
            get
            {
                var result = Filename.SeoFormat().RemoveSpecialCharacters(true).ToLower();

                return result;
            }
        }

        public String Url
        {
            get
            {
                var result = String.Concat(Filename, @"\", "detail.html").ToLower();

                if (!CurrentRoot.IsNullOrEmpty())
                {
                    result = String.Format("http://www.ghostpubs.com/haunted-pubs{0}/",
                        result.Replace(CurrentRoot.ToLower(), String.Empty).Replace("\\", "/"));
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