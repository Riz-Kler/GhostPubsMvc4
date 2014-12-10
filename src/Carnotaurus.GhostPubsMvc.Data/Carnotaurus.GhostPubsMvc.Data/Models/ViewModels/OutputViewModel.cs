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
                        Filename = x.UniqueName.Dashify()
                    }
                    : null).OrderBy(x => x.Text).ToList(),
                MetaDescription = string.Format("Haunted pubs in {0}",
                    regions.Select(region => region.Name).OxbridgeAnd())
                    .SeoMetaDescriptionTruncate(),
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
                Filename = currentRegion.UniqueName,
                JumboTitle = currentRegion.Name,
                Action = PageTypeEnum.Region,
                PageLinks = countyLinks.Select(x => x.Text != null
                    ? new PageLinkModel(currentRoot)
                    {
                        Text = x.Text,
                        Title = x.Text,
                        Filename =
                            string.Format(@"\{0}\{1}", currentRegion.Name.Dashify(),
                                x.Text.Dashify())
                    }
                    : null).OrderBy(x => x.Text).ToList(),
                MetaDescription = string.Format("Haunted pubs in {0}", countyLinks.Select(x => x.Text).OxbridgeAnd())
                .SeoMetaDescriptionTruncate(),
                ArticleDescription = string.Format("Haunted pubs in {0}", countyLinks.Select(x => x.Text).OxbridgeAnd()),
                Parent =
                   new KeyValuePair<string, string>(currentRegion.Name,
                       currentRegion.Name.Dashify().ToLower()),
                Total = orgsInRegionCount,
                Priority = PageTypePriority.Region,
                Previous = history.LastOrDefault(x => x.Action == PageTypeEnum.Region),
                Lineage = new Breadcrumb
                {
                    Region = new PageLinkModel(currentRoot)
                    {
                        Filename = currentRegion.UniqueName,
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
                Filename = authority.UniqueName.Dashify(),
                JumboTitle = authority.Name,
                Action = PageTypeEnum.County,
                PageLinks = townLinks.Select(linkModel => linkModel.Text != null
                    ? new PageLinkModel(currentRoot)
                    {
                        Text = linkModel.Text,
                        Title = linkModel.Text,
                        Filename =
                            string.Format(@"\{0}\{1}\{2}",
                                authority.ParentAuthority.Name.Dashify(),
                                authority.Name.Dashify(),
                                linkModel.Text.Dashify()
                                )
                    }
                    : null).OrderBy(x => x.Text).ToList(),
                MetaDescription = string.Format(
                    "Haunted pubs in {0}",
                    townLinks.Select(x => x.Text).OxbridgeAnd())
                    .SeoMetaDescriptionTruncate(),
                ArticleDescription = string.Format(
                    "Haunted pubs in {0}",
                    townLinks.Select(x => x.Text).OxbridgeAnd()),
                Parent = new KeyValuePair<string, string>(authority.ParentAuthority.Name,
                    authority.ParentAuthority.Name.Dashify().ToLower()),
                Total = count,
                Priority = PageTypePriority.County,
                Previous = history.LastOrDefault(x => x.Action == PageTypeEnum.County),
                Lineage = new Breadcrumb
                {
                    Region = new PageLinkModel(currentRoot)
                    {
                        Filename = authority.ParentAuthority.UniqueName,
                        Id = authority.ParentAuthority.Id,
                        Text = authority.ParentAuthority.Name,
                        Title = authority.ParentAuthority.Name
                    },
                    County = new PageLinkModel(currentRoot)
                    {
                        Filename = authority.UniqueName,
                        Id = authority.Id,
                        Text = authority.Name,
                        Title = authority.Name
                    }
                }
            };
            return countyModel;
        }

        public static OutputViewModel CreateTownOutputViewModel(string town,

            Authority authority,
            IList<PageLinkModel> pubLinks,
            string townPath, string currentRoot, IEnumerable<OutputViewModel> history)
        {
            var townModel = new OutputViewModel(currentRoot)
            {
                Filename = (town + " in " + authority.Name).Dashify(),
                JumboTitle = town + " in " + authority.Name,
                Action = PageTypeEnum.Town,
                PageLinks = pubLinks.Select(x => x.Text != null
                    ? new PageLinkModel(currentRoot)
                    {
                        Text = x.Text,
                        Title = x.Title,
                        Filename =
                            string.Format(@"\{0}\{1}\{2}\{3}\{4}", authority.ParentAuthority.Name.Dashify(),
                                authority.Name.Dashify(),
                                town.Dashify(), x.Id, x.Text.Dashify())
                    }
                    : null).OrderBy(x => x.Text).ToList(),
                MetaDescription = string.Format("{0}, {1}, {2}", town, authority.Name, authority.ParentAuthority.Name)
                .SeoMetaDescriptionTruncate(),
                ArticleDescription = string.Format("{0}, {1}, {2}", town, authority.Name, authority.ParentAuthority.Name),
                Parent = new KeyValuePair<string, string>(authority.ParentAuthority.Name, String.Empty),
                Total = pubLinks.Count(),
                Priority = PageTypePriority.Town,
                Previous = history.LastOrDefault(x => x.Action == PageTypeEnum.Town),
                Lineage = new Breadcrumb
                {
                    Region = new PageLinkModel(currentRoot)
                    {
                        Filename = authority.ParentAuthority.UniqueName,
                        Id = authority.ParentAuthority.Id,
                        Text = authority.ParentAuthority.Name,
                        Title = authority.ParentAuthority.Name
                    },
                    County = new PageLinkModel(currentRoot)
                    {
                        Filename = authority.UniqueName,
                        Id = authority.Id,
                        Text = authority.Name,
                        Title = authority.Name
                    },
                    Town = new PageLinkModel(currentRoot)
                    {
                        Filename = (town + " in " + authority.Name).Dashify(),
                        Id = authority.Id,
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
                Filename = pub.Filename,
                JumboTitle = pub.Title,
                Action = action,
                PageLinks = notes,
                MetaDescription =  pub.DescriptionFromNotes,
                ArticleDescription = string.Format("{0}, {1}", pub.Address, pub.PostcodePrimaryPart),
                Parent = new KeyValuePair<string, string>(pub.Town, pub.Town.Dashify().ToLower()),
                Tags = pub.Sections,
                Priority = PageTypePriority.Pub,
                Previous = history.LastOrDefault(x => x.Action == action),
                Lat = pub.Lat.ToString(),
                Lon = pub.Lon.ToString(),
                OtherNames = pub.Authority.Orgs
                    .Where(x => x.Address == pub.Address 
                        && x.Postcode == pub.Postcode 
                        && x.Id != pub.Id)
                    .Select(
                        org => new PageLinkModel(currentRoot)
                        {
                            Id = org.Id,
                            Text = org.TradingName,
                            Title = org.TradingName,
                            Filename = org.Filename
                        }
                    ).ToList(),
                Lineage = new Breadcrumb
                {
                    Region = new PageLinkModel(currentRoot)
                    {
                        Filename = pub.Authority.ParentAuthority.UniqueName,
                        Id = pub.Id,
                        Text = pub.Authority.ParentAuthority.Name,
                        Title = pub.Authority.ParentAuthority.Name
                    },
                    County = new PageLinkModel(currentRoot)
                    {
                        Filename = pub.Authority.UniqueName,
                        Id = pub.Id,
                        Text = pub.Authority.Name,
                        Title = pub.Authority.Name
                    },
                    Town = new PageLinkModel(currentRoot)
                    {
                        Filename = (pub.Town + " in " + pub.Authority.Name).Dashify(),
                        Id = pub.Id,
                        Text = pub.Town,
                        Title = pub.Town
                    },
                    Pub = new PageLinkModel(currentRoot)
                    {
                        Filename = pub.Filename,
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
            List<PageLinkModel> links, string title, string currentRoot)
        {
            var model = new OutputViewModel(currentRoot)
            {
                JumboTitle = title,
                Action = pageType,
                MetaDescription = description.SeoMetaDescriptionTruncate(),
                ArticleDescription = description,
                Filename = pageType.ToString(),
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
                var result = Filename.Dashify().RemoveSpecialCharacters(true).ToLower();

                return result;
            }
        }

        public String Url
        {
            get
            {
                var result = string.Format("http://www.ghostpubs.com/haunted-pubs/{0}.html", Filename);

                result = result.Replace("\\", "/");

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