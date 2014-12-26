using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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

        public static OutputViewModel CreateAllUkRegionsOutputViewModel(string currentRoot,
            List<PageLinkModel> pageLinks, string metaDescription, string articleDescription)
        {
            if (currentRoot == null) throw new ArgumentNullException("currentRoot");
            if (pageLinks == null) throw new ArgumentNullException("pageLinks");
            if (metaDescription == null) throw new ArgumentNullException("metaDescription");
            if (articleDescription == null) throw new ArgumentNullException("articleDescription");

            var viewModel = new OutputViewModel
            {
                Filename = "UK",
                PageTitle = "Haunted pubs in UK by region",
                JumboTitle = "Haunted pubs in UK by region",
                Action = PageTypeEnum.Country,
                PageLinks = pageLinks,
                MetaDescription = metaDescription,
                ArticleDescription = articleDescription,
                Priority = PageTypePriority.Country,
            };

            return viewModel;
        }

        public static OutputViewModel CreateRegionOutputViewModel(Authority region,
            int orgsInRegionCount, IList<PageLinkModel> authorityLinks,
            PageLinkModel next)
        {
            if (region == null) throw new ArgumentNullException("region");
            if (authorityLinks == null) throw new ArgumentNullException("authorityLinks");
            if (next == null) throw new ArgumentNullException("next");

            var regionModel = new OutputViewModel
            {
                Filename = region.QualifiedName,
                PageTitle = region.Name,
                JumboTitle = region.Name,
                Action = PageTypeEnum.Region,
                PageLinks = authorityLinks.Select(x => x.Text != null
                    ? new PageLinkModel
                    {
                        Text = x.Text,
                        Title = x.Text,
                        Filename = x.Filename
                    }
                    : null).OrderBy(x => x.Text).ToList(),
                MetaDescription = string.Format("Haunted pubs in {0}", authorityLinks.Select(x => x.Text).OxfordAnd())
                    .SeoMetaDescriptionTruncate(),
                ArticleDescription =
                    string.Format("Haunted pubs in {0}", authorityLinks.Select(x => x.Text).OxfordAnd()),
                Total = orgsInRegionCount,
                Priority = PageTypePriority.Region,
                Next = next,
                Lineage = new Breadcrumb
                {
                    Region = new PageLinkModel
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
            PageLinkModel next)
        {
            Breadcrumb lineage;

            if (authority.IsExcluded)
            {
                lineage = new Breadcrumb
              {
                  Region = new PageLinkModel
                  {
                      Filename = authority.QualifiedName,
                      Id = authority.Id,
                      Text = authority.Name,
                      Title = authority.Name
                  },
              };
            }
            else
            {
                lineage = new Breadcrumb
                {
                    Region = new PageLinkModel
                    {
                        Filename = authority.ParentAuthority.QualifiedName,
                        Id = authority.ParentAuthority.Id,
                        Text = authority.ParentAuthority.Name,
                        Title = authority.ParentAuthority.Name
                    },
                    Authority = new PageLinkModel
                    {
                        Filename = authority.QualifiedName,
                        Id = authority.Id,
                        Text = authority.Name,
                        Title = authority.Name
                    }
                };

            }

            if (authority == null) throw new ArgumentNullException("authority");
            if (locations == null) throw new ArgumentNullException("locations");
            if (next == null) throw new ArgumentNullException("next");
            var model = new OutputViewModel
            {
                // this is for example: cheshire-west-and-chester-ua.html
                Filename = authority.QualifiedName.Dashify(),
                JumboTitle = authority.Name,
                PageTitle = authority.Name,
                Action = PageTypeEnum.Authority,
                PageLinks = locations.OrderBy(x => x.Text).ToList(),
                MetaDescription = string.Format(
                    "Haunted pubs in {0}",
                    locations.Select(x => x.Text).OxfordAnd())
                    .SeoMetaDescriptionTruncate(),
                ArticleDescription = string.Format(
                    "Haunted pubs in {0}",
                    locations.Select(x => x.Text).OxfordAnd()),
                Total = count,
                Priority = PageTypePriority.Authority,
                Next = next,
                Lineage = lineage
            };

            return model;
        }

        public static OutputViewModel CreateLocalityOutputViewModel(string locality,
            Authority authority,
            IList<PageLinkModel> orgLinks,
            PageLinkModel next)
        {
            if (locality == null) throw new ArgumentNullException("locality");
            if (authority == null) throw new ArgumentNullException("authority");
            if (orgLinks == null) throw new ArgumentNullException("orgLinks");
            if (next == null) throw new ArgumentNullException("next");

            var model = new OutputViewModel
            {
                // dpc - example: duddon-in-cheshire-west-and-chester-ua.html
                Filename = locality.InDashifed(authority.QualifiedName),
                JumboTitle = locality.In(authority.QualifiedName),
                PageTitle = locality.In(authority.QualifiedName),
                Action = PageTypeEnum.Locality,
                PageLinks = orgLinks.Select(x => x.Text != null
                    ? new PageLinkModel
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
                Next = next,
                Lineage = new Breadcrumb
                {
                    Region = new PageLinkModel
                    {
                        Filename = authority.ParentAuthority.QualifiedName,
                        Id = authority.ParentAuthority.Id,
                        Text = authority.ParentAuthority.Name,
                        Title = authority.ParentAuthority.Name
                    },
                    Authority = new PageLinkModel
                    {
                        Filename = authority.QualifiedName,
                        Id = authority.Id,
                        Text = authority.Name,
                        Title = authority.Name
                    },
                    Locality = new PageLinkModel
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
            PageLinkModel next)
        {
            if (org == null) throw new ArgumentNullException("org");
            if (next == null) throw new ArgumentNullException("next");

            var notes = org.Notes.Select(note => new PageLinkModel
            {
                Id = note.Id,
                Text = note.Text,
                Title = note.Text
            }).ToList();

            const PageTypeEnum action = PageTypeEnum.Pub;

            var model = new OutputViewModel
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
                Next = next,
                Lat = org.Lat.ToString(),
                Lon = org.Lon.ToString(),
                OtherNames = org.Authority.Orgs
                    .Where(o => o.Address == org.Address
                                && o.Postcode == org.Postcode
                                && o.Id != org.Id)
                    .Select(
                        o => new PageLinkModel
                        {
                            Id = o.Id,
                            Text = o.TradingName,
                            Title = o.TradingName,
                            Filename = o.Filename
                        }
                    ).ToList(),
                Lineage = new Breadcrumb
                {
                    Region = new PageLinkModel
                    {
                        Filename = org.Authority.ParentAuthority.QualifiedName,
                        Id = org.Id,
                        Text = org.Authority.ParentAuthority.Name,
                        Title = org.Authority.ParentAuthority.Name
                    },
                    Authority = new PageLinkModel
                    {
                        Filename = org.Authority.QualifiedName,
                        Id = org.Id,
                        Text = org.Authority.Name,
                        Title = org.Authority.Name
                    },
                    Locality = new PageLinkModel
                    {
                        Filename = org.QualifiedLocalityDashified
                            .Dashify(),
                        Id = org.Id,
                        Text = org.Locality,
                        Title = org.Locality
                    },
                    Pub = new PageLinkModel
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
            List<PageLinkModel> links, string title)
        {
            if (priority == null) throw new ArgumentNullException("priority");
            if (description == null) throw new ArgumentNullException("description");
            if (title == null) throw new ArgumentNullException("title");

            var model = new OutputViewModel(true)
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

        public OutputViewModel()
        {
            PageLinks = new List<PageLinkModel>();
            Tags = new List<String>();
            IsStandardLink = false;
        }

        public OutputViewModel(bool isStandardLink)
        {
            PageLinks = new List<PageLinkModel>();
            Tags = new List<String>();
            IsStandardLink = isStandardLink;
        }

        public bool IsStandardLink { get; set; }


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

                var pattern = IsStandardLink // CurrentRoot.Contains(@"\uk\")
                    ? "http://www.ghostpubs.com/haunted-pubs/uk/{0}.html"
                    : "http://www.ghostpubs.com/haunted-pubs/{0}.html";

                var url = String.Format(pattern, Filename.Replace("\\", "/"));

                return url.Dashify().ToLower();
            }
        }

        public IEnumerable<String> Tags { get; set; }

        public Int32 Total { get; set; }

        public String Priority { get; set; }

        public PageLinkModel Next { get; set; }

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