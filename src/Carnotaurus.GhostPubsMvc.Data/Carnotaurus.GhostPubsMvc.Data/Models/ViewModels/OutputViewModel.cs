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
            if (currentRoot == null) throw new ArgumentNullException("currentRoot");
            if (regions == null) throw new ArgumentNullException("regions");

            var viewModel = new OutputViewModel(currentRoot)
            {
                Filename = "UK",
                PageTitle = "Haunted pubs in UK by region",
                JumboTitle = "Haunted pubs in UK by region",
                Action = PageTypeEnum.Country,
                PageLinks = regions.Select(x => x.Name != null
                    ? new PageLinkModel(currentRoot)
                    {
                        Text = x.Name,
                        Title = x.Name,
                        Filename = x.QualifiedName.Dashify()
                    }
                    : null).OrderBy(x => x.Text).ToList(),
                MetaDescription = string.Format("Haunted pubs in {0}",
                    regions.Select(region => region.Name).OxbridgeAnd())
                    .SeoMetaDescriptionTruncate(),
                ArticleDescription = string.Format("Haunted pubs in {0}",
                    regions.Select(region => region.Name).OxbridgeAnd()),
                Priority = PageTypePriority.Country,
            };

            return viewModel;
        }

        public static OutputViewModel CreateRegionOutputViewModel(Authority region,
            int orgsInRegionCount, IList<PageLinkModel> authorityLinks, String currentRoot,
             OutputViewModel  history)
        {
            if (region == null) throw new ArgumentNullException("region");
            if (authorityLinks == null) throw new ArgumentNullException("authorityLinks");
            if (currentRoot == null) throw new ArgumentNullException("currentRoot");
            if (history == null) throw new ArgumentNullException("history");

            var regionModel = new OutputViewModel(currentRoot)
            {
                Filename = region.QualifiedName,
                PageTitle = region.Name,
                JumboTitle = region.Name,
                Action = PageTypeEnum.Region,
                PageLinks = authorityLinks.Select(x => x.Text != null
                    ? new PageLinkModel(currentRoot)
                    {
                        Text = x.Text,
                        Title = x.Text,
                        Filename = x.Filename
                    }
                    : null).OrderBy(x => x.Text).ToList(),
                MetaDescription = string.Format("Haunted pubs in {0}", authorityLinks.Select(x => x.Text).OxbridgeAnd())
                    .SeoMetaDescriptionTruncate(),
                ArticleDescription = string.Format("Haunted pubs in {0}", authorityLinks.Select(x => x.Text).OxbridgeAnd()),
                Total = orgsInRegionCount,
                Priority = PageTypePriority.Region,
                Previous = history ,
                Lineage = new Breadcrumb
                {
                    Region = new PageLinkModel(currentRoot)
                    {
                        Filename = region.QualifiedName,
                        Id = region.Id,
                        Text = region.Name,
                        Title = region.Name,
                    },
                }
            };

            return regionModel;
        }

        public static OutputViewModel CreateAuthorityOutputViewModel(Authority authority, int count,
            IList<PageLinkModel> locations,
            String currentRoot,
             OutputViewModel history)
        {
            if (authority == null) throw new ArgumentNullException("authority");
            if (locations == null) throw new ArgumentNullException("locations");
            if (currentRoot == null) throw new ArgumentNullException("currentRoot");
            if (history == null) throw new ArgumentNullException("history");
            var model = new OutputViewModel(currentRoot)
            {
                // this is for example: cheshire-west-and-chester-ua.html
                Filename = authority.QualifiedName.Dashify(),
                JumboTitle = authority.Name,
                PageTitle = authority.Name,
                Action = PageTypeEnum.Authority,
                PageLinks = locations.OrderBy(x => x.Text).ToList(),
                MetaDescription = string.Format(
                    "Haunted pubs in {0}",
                    locations.Select(x => x.Text).OxbridgeAnd())
                    .SeoMetaDescriptionTruncate(),
                ArticleDescription = string.Format(
                    "Haunted pubs in {0}",
                    locations.Select(x => x.Text).OxbridgeAnd()),
                Total = count,
                Priority = PageTypePriority.Authority,
                Previous = history,
                Lineage = new Breadcrumb
                {
                    Region = new PageLinkModel(currentRoot)
                    {
                        Filename = authority.ParentAuthority.QualifiedName,
                        Id = authority.ParentAuthority.Id,
                        Text = authority.ParentAuthority.Name,
                        Title = authority.ParentAuthority.Name
                    },
                    Authority = new PageLinkModel(currentRoot)
                    {
                        Filename = authority.QualifiedName,
                        Id = authority.Id,
                        Text = authority.Name,
                        Title = authority.Name
                    }
                }
            };

            return model;
        }

        public static OutputViewModel CreateLocalityOutputViewModel(string locality,
            Authority authority,
            IList<PageLinkModel> orgLinks,
            string currentRoot,  OutputViewModel  history)
        {
            if (locality == null) throw new ArgumentNullException("locality");
            if (authority == null) throw new ArgumentNullException("authority");
            if (orgLinks == null) throw new ArgumentNullException("orgLinks");
            if (currentRoot == null) throw new ArgumentNullException("currentRoot");
            if (history == null) throw new ArgumentNullException("history");

            var model = new OutputViewModel(currentRoot)
             {
                 // dpc - example: duddon-in-cheshire-west-and-chester-ua.html
                 Filename = locality.InDashifed(authority.QualifiedName),
                 JumboTitle = locality.In(authority.QualifiedName),
                 PageTitle = locality.In(authority.QualifiedName),
                 Action = PageTypeEnum.Locality,
                 PageLinks = orgLinks.Select(x => x.Text != null
                     ? new PageLinkModel(currentRoot)
                     {
                         Text = x.Text,
                         Title = x.Title,
                         // dpc - example: 10930-the-headless-woman-duddon.html
                         Filename = x.Filename
                     }
                     : null).OrderBy(x => x.Text).ToList(),
                 MetaDescription =
                     string.Format("{0}, {1}, {2}", locality, authority.Name, authority.ParentAuthority.Name)
                         .SeoMetaDescriptionTruncate(),
                 ArticleDescription =
                     string.Format("{0}, {1}, {2}", locality, authority.Name, authority.ParentAuthority.Name),
                 Total = orgLinks.Count(),
                 Priority = PageTypePriority.Locality,
                 Previous = history ,
                 Lineage = new Breadcrumb
                 {
                     Region = new PageLinkModel(currentRoot)
                     {
                         Filename = authority.ParentAuthority.QualifiedName,
                         Id = authority.ParentAuthority.Id,
                         Text = authority.ParentAuthority.Name,
                         Title = authority.ParentAuthority.Name
                     },
                     Authority = new PageLinkModel(currentRoot)
                     {
                         Filename = authority.QualifiedName,
                         Id = authority.Id,
                         Text = authority.Name,
                         Title = authority.Name
                     },
                     Locality = new PageLinkModel(currentRoot)
                     {
                         Filename = locality.InDashifed(authority.QualifiedName),
                         Id = authority.Id,
                         Text = locality,
                         Title = locality
                     }
                 }
             };

            return model;
        }

        public static OutputViewModel CreateOrgOutputViewModel(Org org,
            String currentRoot,
             OutputViewModel history)
        {
            if (org == null) throw new ArgumentNullException("org");
            if (currentRoot == null) throw new ArgumentNullException("currentRoot");
            if (history == null) throw new ArgumentNullException("history");

            var notes = org.Notes.Select(note => new PageLinkModel(currentRoot)
            {
                Id = note.Id,
                Text = note.Text,
                Title = note.Text
            }).ToList();

            const PageTypeEnum action = PageTypeEnum.Pub;

            var model = new OutputViewModel(currentRoot)
            {
                Filename = org.Filename,
                JumboTitle = org.JumboTitle,
                PageTitle = org.Title,
                Action = action,
                PageLinks = notes,
                MetaDescription = org.DescriptionFromNotes,
                ArticleDescription = string.Format("{0}, {1}", org.Address, org.PostcodePrimaryPart),
                Tags = org.Sections,
                Priority = PageTypePriority.Pub,
                Previous = history ,
                Lat = org.Lat.ToString(),
                Lon = org.Lon.ToString(),
                OtherNames = org.Authority.Orgs
                    .Where(o=> o.Address == org.Address
                                && o.Postcode == org.Postcode
                                && o.Id != org.Id)
                    .Select(
                        o => new PageLinkModel(currentRoot)
                        {
                            Id = o.Id,
                            Text = o.TradingName,
                            Title = o.TradingName,
                            Filename = o.Filename
                        }
                    ).ToList(),
                Lineage = new Breadcrumb
                {
                    Region = new PageLinkModel(currentRoot)
                    {
                        Filename = org.Authority.ParentAuthority.QualifiedName,
                        Id = org.Id,
                        Text = org.Authority.ParentAuthority.Name,
                        Title = org.Authority.ParentAuthority.Name
                    },
                    Authority = new PageLinkModel(currentRoot)
                    {
                        Filename = org.Authority.QualifiedName,
                        Id = org.Id,
                        Text = org.Authority.Name,
                        Title = org.Authority.Name
                    },
                    Locality = new PageLinkModel(currentRoot)
                    {
                        Filename = org.QualifiedLocalityDashified
                            .Dashify(),
                        Id = org.Id,
                        Text = org.Locality,
                        Title = org.Locality
                    },
                    Pub = new PageLinkModel(currentRoot)
                    {
                        Filename = org.Filename,
                        Id = org.Id,
                        Text = org.TradingName,
                        Title = org.TradingName
                    }
                }
            };

            return model;
        }

        public static OutputViewModel CreatePageTypeOutputViewModel(PageTypeEnum pageType, string priority,
            string description,
            List<PageLinkModel> links, string title, string currentRoot)
        {
            if (priority == null) throw new ArgumentNullException("priority");
            if (description == null) throw new ArgumentNullException("description");
            if (title == null) throw new ArgumentNullException("title");
            if (currentRoot == null) throw new ArgumentNullException("currentRoot");

            var model = new OutputViewModel(currentRoot)
            {
                JumboTitle = title,
                PageTitle = title,
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

        public OutputViewModel(String currentRoot)
        {
            PageLinks = new List<PageLinkModel>();
            Tags = new List<String>();
            CurrentRoot = currentRoot;
        }

        public String CurrentRoot { get; set; }

        public String Lat { get; set; }

        public String Lon { get; set; }

        public Breadcrumb Lineage { get; set; }

        public String MetaDescription { get; set; }

        public String ArticleDescription { get; set; }

        public String Link { get; set; }

        public String Tag { get; set; }

        public IList<PageLinkModel> OtherNames { get; set; }

        public IList<PageLinkModel> PageLinks { get; set; }

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
                if (Filename.IsNullOrEmpty()) return String.Empty;

                var pattern = CurrentRoot.Contains(@"\uk\")
                    ? "http://www.ghostpubs.com/haunted-pubs/uk/{0}.html"
                    : "http://www.ghostpubs.com/haunted-pubs/{0}.html";

                var url = String.Format(pattern, Filename.Replace("\\", "/"));

                return url.Dashify().ToLower();
            }
        }

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

        public String Filename { get; set; }

        public String JumboTitle { get; set; }

        public PageTypeEnum Action { get; set; }

        public string PageTitle { get; set; }

    }
}