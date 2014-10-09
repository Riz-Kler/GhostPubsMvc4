using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Carnotaurus.GhostPubsMvc.Common.Bespoke.Enumerations;
using Carnotaurus.GhostPubsMvc.Common.Extensions;
using Carnotaurus.GhostPubsMvc.Data.Models.Entities;
using Carnotaurus.GhostPubsMvc.Data.Models.ViewModels;
using Carnotaurus.GhostPubsMvc.Managers.Interfaces;
using Humanizer;

namespace Carnotaurus.GhostPubsMvc.Controllers
{
    public class TemplateController : Controller
    {
        private readonly ICommandManager _commandManager;

        private readonly IQueryManager _queryManager;

        private String _currentRoot = String.Empty;
        private List<OutputViewModel> _history;

        public TemplateController(IQueryManager queryManager, ICommandManager commandManager)
        {
            _commandManager = commandManager;

            _queryManager = queryManager;

            _history = new List<OutputViewModel>();
        }

        public static void DeleteDirectory(string targetDir)
        {
            var files = Directory.GetFiles(targetDir);

            var dirs = Directory.GetDirectories(targetDir);

            foreach (var file in files)
            {
                System.IO.File.SetAttributes(file, FileAttributes.Normal);
                System.IO.File.Delete(file);
            }

            foreach (var dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(targetDir, false);
        }

        public ActionResult Generate()
        {
            UpdateOrganisations();

            _currentRoot = String.Format(@"C:\Carnotaurus\{0}\haunted-pub", Guid.NewGuid()).ToLower().Underscore().Hyphenate();

            if (_currentRoot != null)
            {
                CreateFolders(_currentRoot);

                DeleteFolders(_currentRoot);

                CreateFolders(_currentRoot);
            }

            var data = GetLeaderboardData();

            CreatePageTypeFile(PageTypeEnum.Sitemap, "Sitemap: Pub leaderboard of most haunted areas in UK",
                PageTypePriority.Sitemap, data);

            CreatePageTypeFile(PageTypeEnum.Submissions, "Call for submissions", PageTypePriority.Submissions);

            CreatePageTypeFile(PageTypeEnum.Promotions, "Who is promoting us this month?", PageTypePriority.Promotions);

            CreatePageTypeFile(PageTypeEnum.Competitions, "Competition - Name Our Ghost", PageTypePriority.Competitions);

            CreatePageTypeFile(PageTypeEnum.Partners, "Want to partner with us?", PageTypePriority.Partners);

            CreatePageTypeFile(PageTypeEnum.Partnerships, "Partnering with Ghost Pubs", PageTypePriority.Partnerships);

            CreatePageTypeFile(PageTypeEnum.FeaturedPartner, "Who are our partners?", PageTypePriority.FeaturedPartner);

            CreatePageTypeFile(PageTypeEnum.Contributors, "Credits to our contributors", PageTypePriority.Contributors);

            CreatePageTypeFile(PageTypeEnum.About, "About Ghost Pubs", PageTypePriority.About);

            CreatePageTypeFile(
                PageTypeEnum.Home,
                "Welcome to Ghost Pubs, pubs with a ghostly difference! We have the largest haunted pub directory. Please make your selection below!",
                PageTypePriority.Home,
                title: "Ghost Pubs"
                );

            CreatePageTypeFile(PageTypeEnum.FaqBrewery, "FAQs that Breweries ask about Ghost Pubs",
                PageTypePriority.FaqBrewery);

            CreatePageTypeFile(PageTypeEnum.FaqPub, "FAQs that Publicans ask about Ghost Pubs", PageTypePriority.FaqPub);

            CreatePageTypeFile(PageTypeEnum.Newsletter, "Sign up for our newsletter here for goodies!",
                PageTypePriority.Newsletter);

            CreatePageTypeFile(PageTypeEnum.Accessibility, "What is our Accessibility Policy?",
                PageTypePriority.Accessibility);

            CreatePageTypeFile(PageTypeEnum.Terms, "Terms and conditions", PageTypePriority.Terms);

            CreatePageTypeFile(PageTypeEnum.ContactUs, "Contact Us", PageTypePriority.ContactUs);

            CreatePageTypeFile(PageTypeEnum.Privacy, "Privacy policy", PageTypePriority.Privacy);

            GenerateHtmlPages();

            GenerateWebmasterToolsXmlSitemap();

            return View();
        }

        private void DeleteFolders(String path)
        {
            DeleteDirectory(path);
            DeleteDirectory(path.Replace("-", "_"));
        }

        private void CreateFolders(String path)
        {
            Directory.CreateDirectory(path.ToLower());
            Directory.CreateDirectory(path.ToLower().Replace("-", "_"));
        }

        private void GenerateWebmasterToolsXmlSitemap()
        {
            //</urlset> 

            //var model = PrepareModel("Generated pages", "generate");
            //this.PrepareView(model);

            // Generate Seo Sitemap
            // generate the Google webmaster tools xml url sitemap
            var sb = new StringBuilder();

            sb.AppendLine(@"<?xml version=""1.0"" encoding=""UTF-8""?>");

            sb.AppendLine(
                @"<urlset xmlns=""http://www.sitemaps.org/schemas/sitemap/0.9"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xsi:schemaLocation=""http://www.sitemaps.org/schemas/sitemap/0.9 http://www.sitemaps.org/schemas/sitemap/0.9/sitemap.xsd"">");

            if (_history != null)
            {
                _history = _history.OrderByDescending(model => model.Priority).ToList();

                foreach (var item in _history)
                {
                    sb.AppendLine(item.SitemapItem);
                }
            }

            sb.AppendLine("</urlset>");

            var fullFilePath = String.Format("{0}/ghostpubs-sitemap.xml", _currentRoot);

            WriteFile(fullFilePath, sb.ToString());
        }

        private void UpdateOrganisations()
        {
            var missingInfoOrgs = _queryManager.GetMissingInfoOrgsToUpdate();

            var isSuccess = ResultTypeEnum.Fail;

            foreach (var missingInfoOrg in missingInfoOrgs)
            {
                var xElement = _queryManager.ReadXElement(missingInfoOrg);

                isSuccess = _commandManager.UpdateOrganisation(missingInfoOrg, xElement);

                if (isSuccess == ResultTypeEnum.Success)
                {
                    var result = xElement.Element("result");

                    var outer = _commandManager.UpdateAdministrativeAreaLevels(result, missingInfoOrg);

                    var match = _queryManager.GetCounty(outer);

                    if (match != null)
                    {
                        _commandManager.UpdateCounty(missingInfoOrg, match);
                    }
                }

                if (isSuccess == ResultTypeEnum.Fail)
                {
                    break;
                }
            }

            if (isSuccess != ResultTypeEnum.Fail)
            {
                _commandManager.Save();
            }
        }


        private IEnumerable<Region> CreateRegionsFile(string currentRoot)
        {
            var regions = _queryManager.GetRegions()
                .ToList();

            var regionsModel = new OutputViewModel(_currentRoot)
            {
                JumboTitle = "Haunted pubs in UK by region",
                Action = PageTypeEnum.Country,
                PageLinks = regions.Select(x => x.Name != null
                    ? new PageLinkModel(_currentRoot)
                    {
                        Text = x.Name,
                        Title = x.Name,
                        Unc = @"\" + x.Name.Underscore().Hyphenate()
                    }
                    : null).OrderBy(x => x.Text).ToList(),
                MetaDescription = string.Format("Ghost pubs in {0}",
                    regions.Select(region => region.Name).OxbridgeAnd()),
                ArticleDescription = string.Format("Ghost pubs in {0}",
                    regions.Select(region => region.Name).OxbridgeAnd()),
                Unc = currentRoot,
                Parent = new KeyValuePair<string, string>("Home page", @"/"),
                Priority = PageTypePriority.Country,
            };

            WriteLines(regionsModel);

            return regions;
        }


        private IEnumerable<County> CreateRegionFile(Region currentRegion, string currentRegionPath,
            Int32 orgsInRegionCount)
        {
            // write region directory
            CreateFolders(currentRegionPath);

            // region file needs knowledge of its counties
            // should be list of counties that have ghost pubs?
            // var countiesInRegion = currentRegion.Counties.ToList();

            var hauntedCountiesInRegion = _queryManager.GetHauntedCountiesInRegion(currentRegion.Id).ToList();

            var countyLinks = hauntedCountiesInRegion.Select(x => new PageLinkModel(_currentRoot)
            {
                Text = x.Description,
                Title = x.Description
            }).ToList();

            var regionModel = new OutputViewModel(_currentRoot)
            {
                JumboTitle = currentRegion.Name,
                Action = PageTypeEnum.Region,
                PageLinks = countyLinks.Select(x => x.Text != null
                    ? new PageLinkModel(_currentRoot)
                    {
                        Text = x.Text,
                        Title = x.Text,
                        Unc = string.Format(@"\{0}\{1}", currentRegion.Name.Underscore().Hyphenate(), x.Text.Underscore().Hyphenate())
                    }
                    : null).OrderBy(x => x.Text).ToList(),
                MetaDescription = string.Format("Ghost pubs in {0}", countyLinks.Select(x => x.Text).OxbridgeAnd()),
                ArticleDescription = string.Format("Ghost pubs in {0}", countyLinks.Select(x => x.Text).OxbridgeAnd()),
                Unc = currentRegionPath,
                Parent =
                    new KeyValuePair<string, string>(currentRegion.Name, currentRegion.Name.Underscore().Hyphenate().ToLower()),
                Total = orgsInRegionCount,
                Priority = PageTypePriority.Region,
                Previous = _history.LastOrDefault(x => x.Action == PageTypeEnum.Region),
                Lineage = new Breadcrumb
                {
                    Region = new PageLinkModel(_currentRoot)
                    {
                        Unc = currentRegionPath,
                        Id = currentRegion.Id,
                        Text = currentRegion.Name,
                        Title = currentRegion.Name,
                    },
                }
            };

            WriteLines(regionModel);

            return hauntedCountiesInRegion;
        }

        private void GenerateHtmlPages()
        {
            var regions = CreateRegionsFile(_currentRoot);

            foreach (var currentRegion in regions)
            {
                var currentRegionPath = BuildPath(_currentRoot, currentRegion.Name);

                var orgsInRegionCount = currentRegion.Counties.Sum(x => x.Orgs.Count(y => y.HauntedStatus == 1));

                if (orgsInRegionCount == 0) continue;

                var countiesInRegion = CreateRegionFile(currentRegion, currentRegionPath, orgsInRegionCount);

                foreach (var currentCounty in countiesInRegion)
                {
                    // county file needs knowledge of its addresses for Town
                    if (currentCounty == null) continue;


                    // write them out backwards (so alphabetical from previous) and keep towns together (so need pub has a better chance to be in the same town) 
                    var orgsInCounty =
                        currentCounty.Orgs.Where(x => x.Town != null && x.HauntedStatus == 1) 
                        .OrderByDescending(org => org.Town)
                        .ThenByDescending(org => org.TradingName)
                        .ToList();
 
                    if (currentRegionPath == null) continue;

                    var currentCountyPath = BuildPath(currentRegionPath, currentCounty.Name);

                    if (currentCountyPath == null) continue;

                    CreateFolders(currentCountyPath);

                    var townsInCounty = orgsInCounty.Select(x => x.Town).Distinct().ToList();

                    CreateCountyFile(currentCounty, currentCountyPath, townsInCounty, currentRegion, orgsInCounty.Count,
                        currentRegionPath);

                    var pubTownLinks = new List<KeyValuePair<String, PageLinkModel>>();
                     
                    CreatePubsFiles(orgsInCounty, currentCountyPath, pubTownLinks);

                    CreateTownFiles(townsInCounty, currentCountyPath, pubTownLinks, currentCounty, currentRegion, currentRegionPath);
                }
            }
        }

        private void CreatePubsFiles(IEnumerable<Org> orgsInCounty, string currentCountyPath, List<KeyValuePair<string, PageLinkModel>> pubTownLinks)
        {
            foreach (var currentOrg in orgsInCounty)
            {
                var currentTownPath = BuildPath(currentCountyPath, currentOrg.Town);

                if (currentTownPath == null) continue;

                CreateFolders(currentTownPath);

                //town file needs knowledge of its pubs, e.g., trading name
                // var pubs = currentAddress .Entities.OrderBy(x => x.TradingName).ToList();
                CreatePubFile(pubTownLinks, currentTownPath, currentOrg);
            }
        }

        private void CreateTownFiles(List<string> townsInCounty, string currentCountyPath, List<KeyValuePair<string, PageLinkModel>> pubTownLinks, County currentCounty,
            Region currentRegion, string currentRegionPath)
        {
// create the town pages
            foreach (var town in townsInCounty)
            {
                CreateTownFile(currentCountyPath, pubTownLinks, town, currentCounty, currentRegion,
                    currentRegionPath);
            }
        }

        private void CreateCountyFile(County currentCounty, string currentCountyPath, IEnumerable<string> towns,
            Region currentRegion, Int32 count, string currentRegionPath)
        {
            var townLinks = towns.Select(x => new PageLinkModel(_currentRoot)
            {
                Text = x,
                Title = x
            }).ToList();

            var countyModel = new OutputViewModel(_currentRoot)
            {
                JumboTitle = currentCounty.Name,
                Action = PageTypeEnum.County,
                PageLinks = townLinks.Select(x => x.Text != null
                    ? new PageLinkModel(_currentRoot)
                    {
                        Text = x.Text,
                        Title = x.Text,
                        Unc =
                            @"\" + currentRegion.Name.Underscore().Hyphenate() + @"\" + currentCounty.Name.Underscore().Hyphenate() + @"\" +
                            x.Text.Underscore().Hyphenate()
                    }
                    : null).OrderBy(x => x.Text).ToList(),
                MetaDescription = string.Format("Ghost pubs in {0}", townLinks.Select(x => x.Text).OxbridgeAnd()),
                ArticleDescription = string.Format("Ghost pubs in {0}", townLinks.Select(x => x.Text).OxbridgeAnd()),
                Unc = currentCountyPath,
                Parent = new KeyValuePair<string, string>(currentRegion.Name,
                    currentRegion.Name.Underscore().Hyphenate().ToLower()),
                Total = count,
                Priority = PageTypePriority.County,
                Previous = _history.LastOrDefault(x => x.Action == PageTypeEnum.County),
                Lineage = new Breadcrumb
                {
                    Region = new PageLinkModel(_currentRoot)
                    {
                        Unc = currentRegionPath,
                        Id = currentRegion.Id,
                        Text = currentRegion.Name,
                        Title = currentRegion.Name
                    },
                    County = new PageLinkModel(_currentRoot)
                    {
                        Unc = currentCountyPath,
                        Id = currentCounty.Id,
                        Text = currentCounty.Name,
                        Title = currentCounty.Name
                    }
                }
            };

            // towns need to know about
            WriteLines(countyModel);
        }

        private void CreatePubFile(ICollection<KeyValuePair<string, PageLinkModel>> pubTownLinks, string currentTownPath,
            Org pub)
        {
            pub.TownPath = currentTownPath;

            pubTownLinks.Add(new KeyValuePair<string, PageLinkModel>(pub.Town, pub.ExtractLink(_currentRoot)));

            CreateFolders(pub.Path);

            var notes = pub.Notes.Select(x => new PageLinkModel(_currentRoot)
            {
                Id = x.Id,
                Text = x.Text,
                Title = x.Text
            }).ToList();

            const PageTypeEnum action = PageTypeEnum.Pub;

            var pubModel = new OutputViewModel(_currentRoot)
            {
                JumboTitle = pub.TradingName,
                Action = action,
                PageLinks = notes,
                MetaDescription =
                    string.Format("{0}, {1} : {2}", pub.Address, pub.PostcodePrimaryPart, notes.First().Text),
                ArticleDescription = string.Format("{0}, {1}", pub.Address, pub.PostcodePrimaryPart),
                Unc = pub.Path,
                Parent = new KeyValuePair<string, string>(pub.Town, pub.Town.Underscore().Hyphenate().ToLower()),
                Tags = pub.Tags.Select(x => x.Feature.Name).ToList(),
                Priority = PageTypePriority.Pub,
                Previous = _history.LastOrDefault(x => x.Action == action),
                Lat = pub.Lat.ToString(),
                Lon = pub.Lon.ToString(),
                OtherNames = pub.County.Orgs
                    .Where(x => x.Address == pub.Address && x.Postcode == pub.Postcode && x.Id != pub.Id)
                    .Select(
                        z => new PageLinkModel(_currentRoot)
                        {
                            Id = z.Id,
                            Text = z.TradingName,
                            Title = z.TradingName,
                            Unc =
                                string.Format("{0}/{1}/{2}", pub.TownPath.Underscore().Hyphenate(), z.Id, z.TradingName.Underscore().Hyphenate())
                        }
                    ).ToList(),
                Lineage = new Breadcrumb
                {
                    Region = new PageLinkModel(_currentRoot)
                    {
                        Unc = pub.RegionPath,
                        Id = pub.Id,
                        Text = pub.County.Region.Name,
                        Title = pub.County.Region.Name
                    },
                    County = new PageLinkModel(_currentRoot)
                    {
                        Unc = pub.CountyPath,
                        Id = pub.Id,
                        Text = pub.County.Name,
                        Title = pub.County.Name
                    },
                    Town = new PageLinkModel(_currentRoot)
                    {
                        Unc = pub.TownPath,
                        Id = pub.Id,
                        Text = pub.Town,
                        Title = pub.Town
                    },
                    Pub = new PageLinkModel(_currentRoot)
                    {
                        Unc = pub.Path,
                        Id = pub.Id,
                        Text = pub.TradingName,
                        Title = pub.TradingName
                    }
                }
            };

            WriteLines(pubModel);
        }


        public List<PageLinkModel> GetLeaderboardData()
        {
            var results = _queryManager.GetSitemapData(_currentRoot);

            return results;
        }


        public void CreatePageTypeFile(PageTypeEnum pageType, string description, string priority,
            List<PageLinkModel> links = null, String title = null)
        {
            var path = string.Format("{0}\\{1}", _currentRoot, pageType);

            CreateFolders(path);

            if (title == null)
            {
                title = pageType.ToString().CamelCaseToWords();
            }

            var model = new OutputViewModel(_currentRoot)
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

            WriteLines(model);
        }

        private void CreateTownFile(string currentCountyPath,
            IEnumerable<KeyValuePair<string, PageLinkModel>> pubTownLinks,
            string town,
            County currentCounty, Region currentRegion, string currentRegionPath)
        {
            var townPath = BuildPath(currentCountyPath, town);

            var pubLinks = pubTownLinks
                .Where(x => x.Key.Equals(town))
                .Select(x => x.Value)
                .ToList();

            var townModel = new OutputViewModel(_currentRoot)
            {
                JumboTitle = town,
                Action = PageTypeEnum.Town,
                PageLinks = pubLinks.Select(x => x.Text != null
                    ? new PageLinkModel(_currentRoot)
                    {
                        Text = x.Text,
                        Title = x.Title,
                        Unc =
                            string.Format(@"\{0}\{1}\{2}\{3}\{4}", currentRegion.Name.Underscore().Hyphenate(),
                                currentCounty.Name.Underscore().Hyphenate(),
                                town.Underscore().Hyphenate(), x.Id, x.Text.Underscore().Hyphenate())
                    }
                    : null).OrderBy(x => x.Text).ToList(),
                MetaDescription = string.Format("{0}, {1}, {2}", town, currentCounty.Name, currentRegion.Name),
                ArticleDescription = string.Format("{0}, {1}, {2}", town, currentCounty.Name, currentRegion.Name),
                Unc = townPath,
                Parent = new KeyValuePair<string, string>(currentCounty.Description, String.Empty),
                Total = pubLinks.Count,
                Priority = PageTypePriority.Town,
                Previous = _history.LastOrDefault(x => x.Action == PageTypeEnum.Town),
                Lineage = new Breadcrumb
                {
                    Region = new PageLinkModel(_currentRoot)
                    {
                        Unc = currentRegionPath,
                        Id = currentRegion.Id,
                        Text = currentRegion.Name,
                        Title = currentRegion.Name
                    },
                    County = new PageLinkModel(_currentRoot)
                    {
                        Unc = currentCountyPath,
                        Id = currentCounty.Id,
                        Text = currentCounty.Name,
                        Title = currentCounty.Name
                    },
                    Town = new PageLinkModel(_currentRoot)
                    {
                        Unc = townPath,
                        Id = currentCounty.Id,
                        Text = town,
                        Title = town
                    }
                }
            };

            WriteLines(townModel);
        }

        public String BuildPath(params String[] builder)
        {
            var output = String.Empty;

            foreach (var b in builder)
            {
                if (output == String.Empty)
                {
                    output = b.ToLower().Underscore().Hyphenate();
                }
                else
                {
                    output = string.Format("{0}\\{1}", output, b.ToLower().Underscore().Hyphenate());
                }
            }

            return output;
        }


        protected String PrepareModel(OutputViewModel data)
        {
            var output = this.PrepareView(data, data.Action.ToString());

            return output;
        }

        public void WriteLines(OutputViewModel model)
        {
            _history.Add(model);

            var contents = PrepareModel(model);

            if (model.Unc == null) return;

            var fullFilePath = String.Concat(model.Unc.ToLower(), @"\", "detail.html");

            System.IO.File.WriteAllText(fullFilePath, contents);

            model.Action = PageTypeEnum.Missing;

            contents = PrepareModel(model);

            if (contents.IsNotNullOrEmpty())
            {
                WriteFile(fullFilePath.Replace("-", "_"), contents);
            }
        }

        public void WriteFile(string fullFilePath, string contents)
        {
            // write file
            System.IO.File.WriteAllText(fullFilePath.ToLower(), contents);

        }
    }
}