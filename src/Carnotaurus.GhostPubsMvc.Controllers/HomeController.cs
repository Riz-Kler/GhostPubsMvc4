using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;
using Carnotaurus.GhostPubsMvc.Common.Bespoke;
using Carnotaurus.GhostPubsMvc.Common.Extensions;
using Carnotaurus.GhostPubsMvc.Common.Helpers;
using Carnotaurus.GhostPubsMvc.Data;
using Carnotaurus.GhostPubsMvc.Data.Models;
using Carnotaurus.GhostPubsMvc.Data.Models.ViewModels;
using Carnotaurus.GhostPubsMvc.Managers.Interfaces;
using RazorEngine;

namespace Carnotaurus.GhostPubsMvc.Controllers
{
    public class HomeController : Controller
    {
     private readonly ICommandManager _commandManager;

        private readonly IQueryManager _queryManager;

        public HomeController(IQueryManager queryManager, ICommandManager commandManager)
        {
            _commandManager = commandManager;

            _queryManager = queryManager;
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
            var entities = new CmsContext();

            UpdateOrganisations(entities);

            entities = new CmsContext();

            GenerateHtmlPages(entities);

            //var model = PrepareModel("Generated pages", "generate");

            // this.PrepareView(model);

            return View();
        }

        private void GenerateHtmlPages(CmsContext entities)
        {
            //var entities = entities2.Orgs. Where(
            //             x => x.Address != null
            //               && x.AddressType != null
            //               && x.AddressType.AddressTypeID == 1
            //               && x.Tags.Any(y => y.Feature.FeatureID == 39)
            //            ).ToList();

            var currentRoot = String.Format(@"C:\Carnotaurus\{0}\haunted_pub", Guid.NewGuid());

            Directory.CreateDirectory(currentRoot);

            DeleteDirectory(currentRoot);

            Directory.CreateDirectory(currentRoot);

            var regions = CreateRegionsFile(currentRoot, entities);

            foreach (var currentRegion in regions)
            {
                var currentRegionPath = BuildPath(currentRoot, currentRegion.Name);

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

        private static void UpdateOrganisations(CmsContext entities)
        {
            var missingInfoOrgs = GetMissingInfoOrgsToUpdate(entities);

            foreach (var missingInfoOrg in missingInfoOrgs)
            {
                var isSuccess = UpdateOrganisation(missingInfoOrg, entities);

                if (isSuccess == ResultTypeEnum.Fail)
                {
                    break;
                }

                entities.SaveChanges();
            }
        }

        private static IEnumerable<Org> GetMissingInfoOrgsToUpdate(CmsContext entities)
        {
            var missingInfoOrgs =
                entities.Orgs
                    .Where(f =>
                        f != null
                        && f.Address != null
                        && f.Postcode != null
                        && f.AddressTypeId == 1
                        && f.CountyId != null
                        && f.Tried == null
                    )
                    .ToList();

            return missingInfoOrgs;
        }

        private static ResultTypeEnum UpdateOrganisation(Org missingInfoOrg, CmsContext entities)
        {
            // source correct address, using google maps api or similar

            // E.G., https://maps.googleapis.com/maps/api/geocode/xml?address=26%20Smithfield,%20London,%20Greater%20London,%20EC1A%209LB,%20uk&sensor=true&key=AIzaSyC2DCdkPGBtsooyft7sX3P9h2f4uQvLQj0

            var key = ConfigurationHelper.GetValueAsString("GoogleMapsApiKey"); // "AIzaSyC2DCdkPGBtsooyft7sX3P9h2f4uQvLQj0";

            var requestUri = ("https://maps.google.com/maps/api/geocode/xml?address="
                              + missingInfoOrg.TradingName
                              + ", "
                              + missingInfoOrg.Address
                              + ", "
                              + missingInfoOrg.Postcode
                              + ", UK&sensor=false&key=" + key);

            var xdoc = XDocument.Load(requestUri);

            var xElement = xdoc.Element("GeocodeResponse");

            var isSuccess = ResultTypeEnum.Fail;

            if (xElement == null || xElement.Value.Contains("OVER_QUERY_LIMIT"))
            {
                return isSuccess;
            }

            missingInfoOrg.GoogleMapData = xElement.ToString();

            missingInfoOrg.Modified = DateTime.Now;

            missingInfoOrg.Tried = 1;

            if (xElement.Value.Contains("ZERO_RESULTS"))
            {
                isSuccess = ResultTypeEnum.NoResults;

                return isSuccess;
            }

            var result = xElement.Element("result");

            if (result == null) return isSuccess;

            isSuccess = ResultTypeEnum.Success;

            UpdateGeocodes(result, missingInfoOrg);

            UpdateLocality(result, missingInfoOrg);

            UpdateTown(result, missingInfoOrg);

            UpdateCounty(result, entities, missingInfoOrg);

            return isSuccess;
        }

        private static void UpdateCounty(XContainer result, CmsContext entities, Org org)
        {
            if (result == null) throw new ArgumentNullException("result");

            var countyResult =
                result.Elements("address_component")
                    .FirstOrDefault(x => x.Value.EndsWith("administrative_area_level_2political"));

            if (countyResult == null || countyResult.FirstNode == null) return;

            var inner = countyResult.FirstNode as XElement;

            if (inner == null) return;

            var outer = inner.Value;

            org.AdministrativeAreaLevel2 = outer;

            var match = entities.Counties.FirstOrDefault(x => x.Name == outer);

            if (match == null) return;

            org.CountyId = match.CountyId;
        }

        private static void UpdateTown(XContainer result, Org org)
        {
            if (result == null) throw new ArgumentNullException("result");

            var townResult =
                result.Elements("address_component").FirstOrDefault(x => x.Value.EndsWith("postal_town"));

            if (townResult == null || townResult.FirstNode == null) return;

            var firstResult = townResult.FirstNode as XElement;

            if (firstResult != null) org.Town = firstResult.Value;
        }


        private static void UpdateLocality(XContainer result, Org org)
        {
            if (result == null) throw new ArgumentNullException("result");

            var match =
                result.Elements("address_component").FirstOrDefault(x => x.Value.EndsWith("localitypolitical"));

            if (match == null || match.FirstNode == null) return;

            var firstResult = match.FirstNode as XElement;

            if (firstResult == null) return;

            org.Locality = firstResult.Value;
        }

        private static void UpdateGeocodes(XContainer result, Org org)
        {
            if (result == null) throw new ArgumentNullException("result");

            var element = result.Element("geometry");

            if (element == null) return;

            var locationElement = element.Element("location");

            if (locationElement == null) return;

            var lat = locationElement.Element("lat");
            if (lat != null)
                org.Lat = Convert.ToDouble(lat.Value);

            var lng = locationElement.Element("lng");
            if (lng != null)
                org.Lon = Convert.ToDouble(lng.Value);
        }

        private IEnumerable<Region> CreateRegionsFile(string currentRoot, CmsContext entities)
        {
            var regions = entities.Regions.ToList();

            var regionsModel = new OrgModel
            {
                JumboTitle = "Haunted pubs in UK by region",
                Action = "country",
                Links = regions.Select(x => new LinkModel {Text = x.Name, Title = x.Name}).OrderBy(x => x.Text).ToList(),
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

            var hauntedCountiesInRegion =
                currentRegion.Counties.Where(x => x.Orgs.Any(y => y.HauntedStatus == 1)).ToList();

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

        private void CreatePubFile(ICollection<KeyValuePair<string, LinkModel>> pubTownLinks, string currentTownPath, Org pub)
        {
            var current = BuildPath(currentTownPath, pub.OrgId.ToString(CultureInfo.InvariantCulture), pub.TradingName);

            var info = new KeyValuePair<String, LinkModel>(pub.Town, new LinkModel
            {
                Text = pub.TradingName,
                Title = pub.TradingName + ", " + pub.Postcode,
                Unc = current,
                Id = pub.OrgId
            });

            pubTownLinks.Add(info);

            Directory.CreateDirectory(current);

            //var many = pubs
            //    .Where(x => x.OrgID.Equals(pub.OrgID))
            //    .Select(x => x.TradingName + " " + x.OrgID)
            //    .ToList();

            var notes = pub.Notes.Select(x => new LinkModel
            {
                Id = x.NoteId,
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

        //public ActionResult Generate2()
        //{
        //    ViewBag.Message = "Your app description page.";

        //    CMSEntities c = new CMSEntities();

        //    var entities = c.Entities.Where(
        //                 x => x.Address != null
        //                   && x.Address.AddressType != null
        //                   && x.Address.AddressType.AddressTypeID == 1
        //                   && x.Tags.Any(y => y.Feature.FeatureID == 39)
        //                ).ToList();

        //    var regionRanks = entities.GroupBy(x => x.Address.County1.Region.Description)
        //        .OrderByDescending(x => x.Count())
        //        .Select(x => new KeyValuePair<String, Int32>(x.Key, x.Count()))
        //        .ToList();

        //    var regions = entities
        //        .Select(x => x.Address.County1.Region)
        //        .Distinct()
        //        .OrderBy(x => x.Description)
        //        .ToList();

        //    // todo - now generate regions pages here

        //    foreach (var region in regions)
        //    {
        //        PrepareModel(region.Name, "region");

        //        // todo - generate counties for each region
        //        var counties = region.Counties.Distinct();

        //        foreach (var county in counties)
        //        {
        //            PrepareModel(county.Description, "county");

        //            var towns = county.Addresses
        //                .Distinct()
        //                .GroupBy(x => x.Town)
        //                .OrderBy(x => x.Key)
        //                .Select(x => x.Key)
        //                .ToList();

        //            foreach (var town in towns)
        //            {
        //                // town
        //                PrepareModel(town, "town");

        //                var addresses = county.Addresses
        //                    .Where(x => x.Town.Equals(town))
        //                    .Distinct()
        //                    .ToList();

        //                // pubs
        //                foreach (var address in addresses)
        //                {
        //                    // pub
        //                    var pub = address.Entities
        //                        .FirstOrDefault();

        //                    if (pub != null)
        //                    {
        //                        PrepareModel(pub.TradingName, "pub");
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    OrgModel model = new OrgModel
        //    {
        //        JumboTitle = "Generated pages",
        //        Action = "generate"
        //    };

        //    var viewModel = PrepareModel(model);

        //    this.PrepareView(viewModel);

        //    return View();
        //}

        protected String PrepareModel(OrgModel data)
        {
            var output = this.PrepareView(data, data.Action);

            return output;
        }

        //private String GetViewHtmlIncludingLayout(Controller controller, Object model, String view, String layoutPath)
        //{
        //    var template = controller.GetViewTemplate(view);

        //    var result = Razor.Parse(template, model);

        //    var output = GetViewHtmlIncludingLayout(result, layoutPath);

        //    return output;
        //}

        //private static String GetViewHtmlIncludingLayout(String viewHtml, String layoutPath)
        //{
        //    var layoutTemplate = System.IO.File.ReadAllText(layoutPath);

        //    var merged = layoutTemplate.Replace("{0}", viewHtml);

        //    return merged;
        //}
          
        public void WriteLines(OrgModel entities)
        {
            var model = PrepareModel(entities);

            // write region file
            System.IO.File.WriteAllText(entities.Unc + @"\" + "detail.html", model);
        }
    }
}