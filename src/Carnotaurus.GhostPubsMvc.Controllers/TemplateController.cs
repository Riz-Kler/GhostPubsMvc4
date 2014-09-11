using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Carnotaurus.GhostPubsMvc.Common.Bespoke.Enumerations;
using Carnotaurus.GhostPubsMvc.Common.Extensions;
using Carnotaurus.GhostPubsMvc.Data.Models;
using Carnotaurus.GhostPubsMvc.Data.Models.ViewModels;
using Carnotaurus.GhostPubsMvc.Managers.Interfaces;
using Humanizer;

namespace Carnotaurus.GhostPubsMvc.Controllers
{
    public class TemplateController : Controller
    {
        private readonly ICommandManager _commandManager;

        private readonly List<OutputViewModel> _history;
        private readonly IQueryManager _queryManager;

        private String _currentRoot = String.Empty;

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

            _currentRoot = String.Format(@"C:\Carnotaurus\{0}\haunted_pub", Guid.NewGuid()).ToLower().Underscore();

            Directory.CreateDirectory(_currentRoot);

            DeleteDirectory(_currentRoot);

            Directory.CreateDirectory(_currentRoot);

            GenerateLeaderboardSitemap();

            CreatePageTypeFile(PageTypeEnum.Submissions, "Call for submissions");

            CreatePageTypeFile(PageTypeEnum.Promotions, "Who is promoting us this month?");

            CreatePageTypeFile(PageTypeEnum.Competitions, "Competition - Name Our Ghost");

            CreatePageTypeFile(PageTypeEnum.Partners, "Want to partner with us?");

            CreatePageTypeFile(PageTypeEnum.Partnerships, "Partnering with GhostPubs.com");

            CreatePageTypeFile(PageTypeEnum.FeaturedPartner, "Who are our partners?");

            CreatePageTypeFile(PageTypeEnum.Contributors, "Credits to our contributors");

            CreatePageTypeFile(PageTypeEnum.About, "About ghost pubs.com");

            CreatePageTypeFile(PageTypeEnum.Home, "Pubs with a ghostly difference");

            // 
            GenerateHtmlPages();

            GenerateWebmasterToolsXmlSitemap();

            return View();
        }

        private void GenerateLeaderboardSitemap()
        {
            var data = GetLeaderboardData();

            CreateLeaderboardFile(data);
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

            foreach (var item in _history)
            {
                sb.AppendLine(item.SitemapItem);
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
            var regions = _queryManager.GetRegions().ToList();

            var regionsModel = new OutputViewModel(_currentRoot)
            {
                JumboTitle = "Haunted pubs in UK by region",
                Action = PageTypeEnum.Country,
                PageLinks = regions.Select(x => x.Name != null
                    ? new PageLinkModel(_currentRoot)
                    {
                        Text = x.Name,
                        Title = x.Name,
                        Unc = @"\" + x.Name.Underscore()
                    }
                    : null).OrderBy(x => x.Text).ToList(),
                Description = string.Format("Ghost pubs in {0}", regions.Select(x => x.Name).OxbridgeAnd()),
                Unc = currentRoot,
                Parent = new KeyValuePair<string, string>("Home page", @"/"),
                Priority = "0.2",
            };

            WriteLines(regionsModel);

            return regions;
        }


        private IEnumerable<County> CreateRegionFile(Region currentRegion, string currentRegionPath,
            Int32 orgsInRegionCount)
        {
            // write region directory
            Directory.CreateDirectory(currentRegionPath);

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
                        Unc = string.Format(@"\{0}\{1}", currentRegion.Name.Underscore(), x.Text.Underscore())
                    }
                    : null).OrderBy(x => x.Text).ToList(),
                Description = string.Format("Ghost pubs in {0}", countyLinks.Select(x => x.Text).OxbridgeAnd()),
                Unc = currentRegionPath,
                Parent =
                    new KeyValuePair<string, string>(currentRegion.Name, currentRegion.Name.Underscore().ToLower()),
                Total = orgsInRegionCount,
                Priority = "0.4",
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

                //if (countiesInRegion == null) continue;

                foreach (var currentCounty in countiesInRegion)
                {
                    // county file needs knowledge of its addresses for Town
                    if (currentCounty == null) continue;

                    var orgsInCounty =
                        currentCounty.Orgs.Where(x => x.Town != null && x.HauntedStatus == 1).ToList();

                    // if (!orgsInCounty.Any()) continue;

                    if (currentRegionPath == null) continue;

                    var currentCountyPath = BuildPath(currentRegionPath, currentCounty.Name);

                    // write county directory
                    if (currentCountyPath == null) continue;

                    Directory.CreateDirectory(currentCountyPath);

                    var townsInCounty = orgsInCounty.Select(x => x.Town).Distinct().ToList();

                    CreateCountyFile(currentCounty, currentCountyPath, townsInCounty, currentRegion, orgsInCounty.Count,
                        currentRegionPath);

                    var pubTownLinks = new List<KeyValuePair<String, PageLinkModel>>();

                    foreach (var currentOrg in orgsInCounty)
                    {
                        var currentTownPath = BuildPath(currentCountyPath, currentOrg.Town);

                        if (currentTownPath == null) continue;

                        Directory.CreateDirectory(currentTownPath);

                        //town file needs knowledge of its pubs, e.g., trading name
                        // var pubs = currentAddress .Entities.OrderBy(x => x.TradingName).ToList();
                        CreatePubFile(pubTownLinks, currentTownPath, currentOrg);
                    }

                    // create the town pages
                    foreach (var town in townsInCounty)
                    {
                        CreateTownFile(currentCountyPath, pubTownLinks, town, currentCounty, currentRegion,
                            currentRegionPath);
                    }
                }
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
                            @"\" + currentRegion.Name.Underscore() + @"\" + currentCounty.Name.Underscore() + @"\" +
                            x.Text.Underscore()
                    }
                    : null).OrderBy(x => x.Text).ToList(),
                Description = "Ghost pubs in " + townLinks.Select(x => x.Text).OxbridgeAnd(),
                Unc = currentCountyPath,
                Parent = new KeyValuePair<string, string>(currentRegion.Name,
                    currentRegion.Name.Underscore().ToLower()),
                Total = count,
                Priority = "0.6",
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

            Directory.CreateDirectory(pub.Path);

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
                Description = string.Format("{0}, {1}", pub.Address, pub.PostcodePrimaryPart),
                Unc = pub.Path,
                Parent = new KeyValuePair<string, string>(pub.Town, pub.Town.Underscore().ToLower()),
                Tags = pub.Tags.Select(x => x.Feature.Name).ToList(),
                Priority = "1.0",
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
                            Unc = pub.TownPath.Underscore() + @"/" + z.Id + @"/" + z.TradingName.Underscore()
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


        public void CreatePageTypeFile(PageTypeEnum pageType, string description)
        {
            var path = string.Format("{0}\\{1}\\", _currentRoot, pageType);

            Directory.CreateDirectory(path);

            var model = new OutputViewModel(_currentRoot)
            {
                JumboTitle = pageType.ToString().CamelCaseToWords(),
                Action = pageType,
                Description = description,
                Unc = path,
                Priority = "0.9",
            };

            WriteLines(model);
        }

        public void CreateLeaderboardFile(List<PageLinkModel> links)
        {
            var path = string.Format("{0}\\{1}\\", _currentRoot, PageTypeEnum.Sitemap);

            Directory.CreateDirectory(path);

            var model = new OutputViewModel(_currentRoot)
            {
                JumboTitle = PageTypeEnum.Sitemap.ToString(),
                Action = PageTypeEnum.Sitemap,
                PageLinks = links,
                Description = "Sitemap: Pub leaderboard of most haunted areas in UK",
                Unc = path,
                Parent = new KeyValuePair<string, string>(String.Empty, String.Empty),
                Total = links.Count(),
                Priority = "0.9",
                Previous = null,
                Lineage = null
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
                        Title = x.Text,
                        Unc =
                            string.Format(@"\{0}\{1}\{2}\{3}\{4}", currentRegion.Name.Underscore(),
                                currentCounty.Name.Underscore(),
                                town.Underscore(), x.Id, x.Text.Underscore())
                    }
                    : null).OrderBy(x => x.Text).ToList(),
                Description = town,
                Unc = townPath,
                Parent = new KeyValuePair<string, string>(currentCounty.Description, String.Empty),
                Total = pubLinks.Count,
                Priority = "0.8",
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
                    output = b.ToLower().Underscore();
                }
                else
                {
                    output = string.Format("{0}\\{1}", output, b.ToLower().Underscore());
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

            WriteFile(fullFilePath, contents);
        }

        public void WriteFile(string fullFilePath, string contents)
        {
            // write file
            System.IO.File.WriteAllText(fullFilePath.ToLower(), contents);
        }
    }
}