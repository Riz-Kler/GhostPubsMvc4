using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Carnotaurus.GhostPubsMvc.Common.Extensions;
using Carnotaurus.GhostPubsMvc.Data.Models;
using Carnotaurus.GhostPubsMvc.Data.Models.ViewModels;
using Carnotaurus.GhostPubsMvc.Managers.Interfaces;

namespace Carnotaurus.GhostPubsMvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICommandManager _commandManager;

        private readonly IQueryManager _queryManager;

        private List<String> _sitemapUrls;

        private String _currentRoot = String.Empty;

        public HomeController(IQueryManager queryManager, ICommandManager commandManager)
        {
            _commandManager = commandManager;

            _queryManager = queryManager;

            _sitemapUrls = new List<String>();
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

            GenerateHtmlPages();

            GenerateWebmasterToolsXmlSitemap();

            return View();
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

            foreach (var url in _sitemapUrls)
            {
                sb.AppendLine(url);
            }

            sb.AppendLine("</urlset>");

            var fullFilePath = String.Format("{0}/sitemap.xml", _currentRoot);

            WriteFile(fullFilePath, sb.ToString());
        }

        private void UpdateOrganisations()
        {
            var missingInfoOrgs = _queryManager.GetMissingInfoOrgsToUpdate();

            _commandManager.UpdateOrgs(missingInfoOrgs);
        }


        private IEnumerable<Region> CreateRegionsFile(string currentRoot)
        {
            var regions = _queryManager.GetRegions().ToList();

            var regionsModel = new OrgModel
            {
                JumboTitle = "Haunted pubs in UK by region",
                Action = "country",
                Links = regions.Select(x => new LinkModel { Text = x.Name, Title = x.Name }).OrderBy(x => x.Text).ToList(),
                Description = "Ghost pubs in " + regions.Select(x => x.Name).OxbridgeAnd(),
                Unc = currentRoot,
                Parent = new KeyValuePair<string, string>("Home page", @"/")
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

            var countyLinks = hauntedCountiesInRegion.Select(x => new LinkModel
            {
                Text = x.Description,
                Title = x.Description
            }).ToList();

            var regionModel = new OrgModel
            {
                JumboTitle = currentRegion.Name,
                Action = "region",
                Links = countyLinks.OrderBy(x => x.Text).ToList(),
                Description = "Ghost pubs in " + countyLinks.Select(x => x.Text).OxbridgeAnd(),
                Unc = currentRegionPath,
                Parent =
                    new KeyValuePair<string, string>(currentRegion.Name, currentRegion.Name.Replace(" ", "_").ToLower()),
                Total = orgsInRegionCount
            };

            WriteLines(regionModel);

            return hauntedCountiesInRegion;
        }

        private void GenerateHtmlPages()
        {
            //var entities = entities2.Orgs. Where(
            //             x => x.Address != null
            //               && x.AddressType != null
            //               && x.AddressType.AddressTypeID == 1
            //               && x.Tags.Any(y => y.Feature.FeatureID == 39)
            //            ).ToList();

            _currentRoot = String.Format(@"C:\Carnotaurus\{0}\haunted_pub", Guid.NewGuid()).ToLower();

            Directory.CreateDirectory(_currentRoot);

            DeleteDirectory(_currentRoot);

            Directory.CreateDirectory(_currentRoot);

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

                    CreateCountyFile(currentCounty, currentCountyPath, townsInCounty, currentRegion, orgsInCounty.Count);

                    var pubTownLinks = new List<KeyValuePair<String, LinkModel>>();

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
                        CreateTownFile(currentCountyPath, pubTownLinks, town, currentCounty);
                    }
                }
            }
        }


        private void CreateCountyFile(County currentCounty, string currentCountyPath, IEnumerable<string> towns,
            Region currentRegion, Int32 count)
        {
            var townLinks = towns.Select(x => new LinkModel
            {
                Text = x,
                Title = x
            }).ToList();

            var countyModel = new OrgModel
            {
                JumboTitle = currentCounty.Name,
                Action = "county",
                Links = townLinks.OrderBy(x => x.Text).ToList(),
                Description = "Ghost pubs in " + townLinks.Select(x => x.Text).OxbridgeAnd(),
                Unc = currentCountyPath,
                Parent = new KeyValuePair<string, string>(currentRegion.Name,
                    currentRegion.Name.Replace(" ", "_").ToLower()),
                Total = count
            };

            // towns need to know about
            WriteLines(countyModel);
        }

        private void CreatePubFile(ICollection<KeyValuePair<string, LinkModel>> pubTownLinks, string currentTownPath,
            Org pub)
        {
            var current = BuildPath(currentTownPath, pub.Id.ToString(CultureInfo.InvariantCulture), pub.TradingName);

            var info = new KeyValuePair<String, LinkModel>(pub.Town, new LinkModel
            {
                Text = pub.TradingName,
                Title = pub.TradingName + ", " + pub.Postcode,
                Unc = current,
                Id = pub.Id
            });

            pubTownLinks.Add(info);

            Directory.CreateDirectory(current);

            //var many = pubs
            //    .Where(x => x.OrgID.Equals(pub.OrgID))
            //    .Select(x => x.TradingName + " " + x.OrgID)
            //    .ToList();

            var notes = pub.Notes.Select(x => new LinkModel
            {
                Id = x.Id,
                Text = x.Text,
                Title = x.Text
            }).ToList();

            var pubModel = new OrgModel
            {
                JumboTitle = pub.TradingName,
                Action = "pub",
                Links = notes,
                Description = pub.Address + ", " + pub.PostcodePrimaryPart,
                Unc = current,
                Parent = new KeyValuePair<string, string>(pub.Town, pub.Town.Replace(" ", "_").ToLower()),
                Tags = pub.Tags.Select(x => x.Feature.Name).ToList()
            };

            WriteLines(pubModel);
        }


        private void CreateTownFile(string currentCountyPath, IEnumerable<KeyValuePair<string, LinkModel>> pubTownLinks,
            string town,
            County currentCounty)
        {
            var townPath = BuildPath(currentCountyPath, town);

            var pubLinks = pubTownLinks
                .Where(x => x.Key.Equals(town))
                .Select(x => x.Value)
                .ToList();

            var townModel = new OrgModel
            {
                JumboTitle = town,
                Action = "town",
                Links = pubLinks,
                Description = town,
                Unc = townPath,
                Parent = new KeyValuePair<string, string>(currentCounty.Description, String.Empty),
                Total = pubLinks.Count
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
                    output = b.ToLower().Replace(" ", "_");
                }
                else
                {
                    output = output + @"\" + b.ToLower().Replace(" ", "_");
                }
            }

            return output;
        }


        protected String PrepareModel(OrgModel data)
        {
            var output = this.PrepareView(data, data.Action);

            return output;
        }

        public void WriteLines(OrgModel entities)
        {
            var contents = PrepareModel(entities);

            if (entities.Unc == null) return;

            var fullFilePath = String.Concat(entities.Unc, @"\", "detail.html");

            WriteFile(fullFilePath, contents);

            // add to webmaster tools sitemap
            AddUrl(fullFilePath);
        }

        public void WriteFile(string fullFilePath, string contents)
        {
            // write file
            System.IO.File.WriteAllText(fullFilePath, contents);
        }

        private void AddUrl(string fullFilePath)
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
                String.Format("http://www.ghostpubs.com/haunted_pub{0}", fullFilePath.Replace(_currentRoot, String.Empty).Replace("\\", "/")),
                DateTime.UtcNow.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'+00:00'"),
                "daily",
                "0.8"
                );

            _sitemapUrls.Add(output);
        }
    }
}