using System;
using System.Collections.Generic;
using System.Linq;
using Carnotaurus.GhostPubsMvc.Common.Bespoke.Enumerations;
using Carnotaurus.GhostPubsMvc.Common.Extensions;
using Carnotaurus.GhostPubsMvc.Data.Interfaces;
using Carnotaurus.GhostPubsMvc.Data.Models.Entities;
using Humanizer;

namespace Carnotaurus.GhostPubsMvc.Data.Models.ViewModels
{
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
                Priority = PageTypePriority.Country
            };

            return viewModel;
        }

        public static OutputViewModel CreateRegionOutputViewModel(Authority region,
            int count,
            IList<PageLinkModel> authorityLinks,
            PageLinkModel next,
            String descriptionPattern)
        {
            if (region == null) throw new ArgumentNullException("region");
            if (authorityLinks == null) throw new ArgumentNullException("authorityLinks");
            if (next == null) throw new ArgumentNullException("next");

            var lineage = new Breadcrumb
            {
                Region = new PageLinkModel
                {
                    Filename = region.CleanQualifiedName,
                    Id = region.Id,
                    Text = region.Name,
                    Title = region.Name,
                    Total = region.HauntedPubCount
                },
            };

            var pageLinks = authorityLinks.Select(link => link.Text != null
                ? new PageLinkModel
                {
                    Text = link.Text,
                    Title = link.Text,
                    Filename = link.Filename,
                    Total = link.Total
                }
                : null)
                .OrderBy(o => o.Text)
                .ToList();

            var articleDescription = string.Format(descriptionPattern, authorityLinks
                .Select(x => x.Text)
                .OxfordAnd());

            var metaDescription = articleDescription
                .SeoMetaDescriptionTruncate();

            var model = new OutputViewModel
            {
                Filename = region.CleanQualifiedName,
                PageTitle = region.Name,
                JumboTitle = region.Name,
                Action = PageTypeEnum.Region,
                Total = count,
                Priority = PageTypePriority.Region,
                Next = next,
                Lineage = lineage,
                PageLinks = pageLinks,
                MetaDescription = metaDescription,
                ArticleDescription = articleDescription,
                Summary = region.Summary,
                LocalGhostSpecialist = region.IsLocalGhostSpecialistValid
                    ? new PageLinkModel
                    {
                        Text = region.LocalGhostSpecialistName,
                        Title = region.LocalGhostSpecialistName,
                        ExternalUrl = region.LocalGhostSpecialistUrl
                    }
                    : null,
                LocalGhostHunter = region.IsLocalGhostHuntSpecialistValid
                    ? new PageLinkModel
                    {
                        Text = region.LocalGhostHuntSpecialistName,
                        Title = region.LocalGhostHuntSpecialistName,
                        ExternalUrl = region.LocalGhostHuntSpecialistUrl
                    }
                    : null
            };

            return model;
        }

        public static OutputViewModel CreateAuthorityOutputViewModel(Authority authority, int count,
            IList<PageLinkModel> locations,
            PageLinkModel next,
            string descriptionPattern
            )
        {
            if (authority == null) throw new ArgumentNullException("authority");
            if (locations == null) throw new ArgumentNullException("locations");
            if (next == null) throw new ArgumentNullException("next");

            var lineage = new Breadcrumb
            {
                Region = new PageLinkModel
                {
                    Filename = authority.ParentAuthority.CleanQualifiedName,
                    Id = authority.ParentAuthority.Id,
                    Text = authority.ParentAuthority.Name,
                    Title = authority.ParentAuthority.Name,
                    Total = authority.ParentAuthority.HauntedPubCount,
                },
                Authority = new PageLinkModel
                {
                    Filename = authority.CleanQualifiedName,
                    Id = authority.Id,
                    Text = authority.Name,
                    Title = authority.Name,
                    Total = authority.HauntedPubCount,
                }
            };

            var adjusted = new Breadcrumb();

            if (authority.IsOutsideUnitedKingdom)
            {
                lineage = adjusted.Swap(lineage);
            }

            var pageLinks = locations.OrderBy(x => x.Text).ToList();

            var articleDescription = string.Format(
                descriptionPattern,
                locations.Select(x => x.Text).OxfordAnd());

            var metaDescription = articleDescription
                .SeoMetaDescriptionTruncate();

            var model = new OutputViewModel
            {
                // this is for example: cheshire-west-and-chester-ua.html
                Filename = authority.CleanQualifiedName,
                JumboTitle = authority.Name,
                PageTitle = authority.Name,
                Action = PageTypeEnum.Authority,
                Total = count,
                Priority = PageTypePriority.Authority,
                Next = next,
                Lineage = lineage,
                MetaDescription = metaDescription,
                ArticleDescription = articleDescription,
                PageLinks = pageLinks,
                Summary = authority.Summary,
                LocalGhostSpecialist = authority.IsLocalGhostSpecialistValid
                    ? new PageLinkModel
                    {
                        Text = authority.LocalGhostSpecialistName,
                        Title = authority.LocalGhostSpecialistName,
                        ExternalUrl = authority.LocalGhostSpecialistUrl
                    }
                    : null,
                LocalGhostHunter = authority.IsLocalGhostHuntSpecialistValid
                    ? new PageLinkModel
                    {
                        Text = authority.LocalGhostHuntSpecialistName,
                        Title = authority.LocalGhostHuntSpecialistName,
                        ExternalUrl = authority.LocalGhostHuntSpecialistUrl
                    }
                    : null
            };

            return model;
        }

        public static OutputViewModel CreateLocalityOutputViewModel(string locality,
            Authority authority,
            IList<PageLinkModel> orgLinks,
            PageLinkModel next,
            String descriptionPattern)
        {
            if (locality == null) throw new ArgumentNullException("locality");
            if (authority == null) throw new ArgumentNullException("authority");
            if (orgLinks == null) throw new ArgumentNullException("orgLinks");
            if (next == null) throw new ArgumentNullException("next");

            var lineage = new Breadcrumb
            {
                Region = new PageLinkModel
                {
                    Filename = authority.ParentAuthority.CleanQualifiedName,
                    Id = authority.ParentAuthority.Id,
                    Text = authority.ParentAuthority.Name,
                    Title = authority.ParentAuthority.Name,
                    Total = authority.ParentAuthority.HauntedPubCount
                },
                Authority = new PageLinkModel
                {
                    Filename = authority.CleanQualifiedName,
                    Id = authority.Id,
                    Text = authority.Name,
                    Title = authority.Name,
                    Total = authority.HauntedPubCount
                },
                Locality = new PageLinkModel
                {
                    Filename = locality.In(authority.CleanQualifiedName, true),
                    Id = authority.Id,
                    Text = locality,
                    Title = locality
                }
            };


            var adjusted = new Breadcrumb();

            if (authority.IsOutsideUnitedKingdom)
            {
                lineage = adjusted.Swap(lineage);
            }

            var articleDescription = string.Format(descriptionPattern, locality, authority.Name,
                authority.ParentAuthority.Name);

            var metaDescription = articleDescription
                .SeoMetaDescriptionTruncate();

            var pageLinks = orgLinks.Select(link => link.Text != null
                ? new PageLinkModel
                {
                    Text = link.Text,
                    Title = link.Title,
                    // dpc - example: 10930-the-headless-woman-duddon.html
                    Filename = link.Filename
                }
                : null)
                .OrderBy(o => o.Text)
                .ToList();

            var model = new OutputViewModel
            {
                // dpc - example: duddon-in-cheshire-west-and-chester-ua.html
                Filename = locality.In(authority.CleanQualifiedName, true),
                JumboTitle = locality.In(authority.QualifiedName),
                PageTitle = locality.In(authority.QualifiedName),
                Action = PageTypeEnum.Locality,
                Total = orgLinks.Count(),
                Priority = PageTypePriority.Locality,
                Next = next,
                Lineage = lineage,
                MetaDescription = metaDescription,
                ArticleDescription = articleDescription,
                PageLinks = pageLinks
            };

            return model;
        }

        public static OutputViewModel CreateOrgOutputViewModel(
            Org org,
            PageLinkModel next,
            String descriptionPattern
            )
        {
            if (org == null) throw new ArgumentNullException("org");
            if (next == null) throw new ArgumentNullException("next");

            //var notes = org.Notes.Select(note => new PageLinkModel
            //{
            //    Id = note.Id,
            //    Text = note.Text,
            //    Title = note.Text
            //}).ToList();

            const PageTypeEnum action = PageTypeEnum.Pub;

            var lineage = new Breadcrumb
            {
                Region = new PageLinkModel
                {
                    Filename = org.Authority.ParentAuthority.CleanQualifiedName,
                    Id = org.Id,
                    Text = org.Authority.ParentAuthority.Name,
                    Title = org.Authority.ParentAuthority.Name,
                    Total = org.Authority.ParentAuthority.HauntedPubCount
                },
                Authority = new PageLinkModel
                {
                    Filename = org.Authority.CleanQualifiedName,
                    Id = org.Id,
                    Text = org.Authority.Name,
                    Title = org.Authority.Name,
                    Total = org.Authority.HauntedPubCount
                },
                Locality = new PageLinkModel
                {
                    Filename = org.QualifiedLocalityDashified
                        .Clean(),
                    Id = org.Id,
                    Text = org.Locality,
                    Title = org.Locality
                },
                Organisation = new PageLinkModel
                {
                    Filename = org.Filename,
                    Id = org.Id,
                    Text = org.TradingName,
                    Title = org.TradingName
                }
            };

            var adjusted = new Breadcrumb();

            if (org.IsOutsideUnitedKingdom)
            {
                lineage = adjusted.Swap(lineage);
            }

            var model = new OutputViewModel
            {
                IsMapAvailable = !org.IsOutsideUnitedKingdom,
                Filename = org.Filename,
                JumboTitle = org.JumboTitle,
                PageTitle = org.Title,
                Action = action,
                PageLinks = new List<PageLinkModel>(),
                MetaDescription = org.SeoDescription,
                Description = org.Description,
                ArticleDescription = string.Format(descriptionPattern, org.Address, org.PostcodePrimaryPart),
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
                Lineage = lineage
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

        public bool IsMapAvailable { get; set; }

        public String Lat { get; set; }

        public String Lon { get; set; }

        public Breadcrumb Lineage { get; set; }

        public String MetaDescription { get; set; }

        public String Description { get; set; }

        public String ArticleDescription { get; set; }

        public String Link { get; set; }

        public String Tag { get; set; }

        public IList<PageLinkModel> OtherNames { get; set; }

        public IList<PageLinkModel> PageLinks { get; set; }

        public String FriendlyFilename
        {
            get
            {
                var result = Filename.Clean().RemoveSpecialCharacters(true);

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

                return url;
            }
        }

        public IEnumerable<String> Tags { get; set; }

        public String TotalInWords
        {
            get { return Total.ToWords(); }
        }

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

        public string Summary { get; set; }

        public PageLinkModel LocalGhostSpecialist { get; set; }

        public bool IsLocalGhostSpecialistAvailable
        {
            get { return LocalGhostSpecialist != null; }
        }


        public PageLinkModel LocalGhostHunter { get; set; }

        public bool IsLocalGhostHunterAvailable
        {
            get { return LocalGhostHunter != null; }
        }

        public String Filename { get; set; }

        public String JumboTitle { get; set; }

        public PageTypeEnum Action { get; set; }

        public string PageTitle { get; set; }
    }
}