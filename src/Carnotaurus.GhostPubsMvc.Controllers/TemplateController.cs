﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;
using Carnotaurus.GhostPubsMvc.Common.Bespoke;
using Carnotaurus.GhostPubsMvc.Common.Bespoke.Enumerations;
using Carnotaurus.GhostPubsMvc.Common.Extensions;
using Carnotaurus.GhostPubsMvc.Common.Helpers;
using Carnotaurus.GhostPubsMvc.Data.Models.Entities;
using Carnotaurus.GhostPubsMvc.Data.Models.ViewModels;
using Carnotaurus.GhostPubsMvc.Managers.Implementation;
using Carnotaurus.GhostPubsMvc.Managers.Interfaces;

namespace Carnotaurus.GhostPubsMvc.Controllers
{
    public class TemplateController : Controller
    {
        private readonly ICommandManager _commandManager;

        private readonly List<OutputViewModel> _history;
        private readonly List<String> _historySitemap;
        private readonly IQueryManager _queryManager;
        private readonly IThirdPartyApiManager _thirdPartyApiManager;

        private String _currentRoot = String.Empty;
        private Guid _generationId;
        private Boolean _isDeprecated;

        public TemplateController(IQueryManager queryManager, ICommandManager commandManager,
            IThirdPartyApiManager thirdPartyApiManager)
        {
            _commandManager = commandManager;

            _queryManager = queryManager;

            _thirdPartyApiManager = thirdPartyApiManager;

            _history = new List<OutputViewModel>();

            _historySitemap = new List<String>();
        }

        public ActionResult Generate()
        {
            _generationId = Guid.NewGuid();

            GenerateLiveContent();

            return View();
        }

        private void GenerateLiveContent()
        {
            _isDeprecated = false;

            var orgsToUpdate = _queryManager.GetOrgsToUpdate();

            UpdateOrganisations(orgsToUpdate);

            GenerateContent();

            GenerateSimpleHtmlPages();

            GenerateLeaderboard();

            GenerateWebmasterSitemap();
        }

        private void GenerateWebmasterSitemap()
        {
            var sm = _queryManager.PrepareWebmasterSitemap(_historySitemap);

            var fullFilePath = String.Format("{0}/ghostpubs-sitemap.xml", _currentRoot);

            FileSystemHelper.WriteFile(fullFilePath, sm);
        }

        private void GenerateDeadContent()
        {
            _isDeprecated = true;

            GenerateContent();
        }

        private void GenerateContent()
        {
            _currentRoot = String.Format(@"C:\Carnotaurus\{0}\haunted-pubs",
                _generationId.ToString().ToLower().SeoFormat());

            if (_currentRoot != null)
            {
                FileSystemHelper.EnsureFolders(_currentRoot, _isDeprecated);
            }

            GenerateGeographicHtmlPages();
        }

        private void GenerateSimpleHtmlPages()
        {
            CreatePageTypeFile(PageTypeEnum.Submissions, PageTypePriority.Submissions, "Call for submissions");

            CreatePageTypeFile(PageTypeEnum.Promotions, PageTypePriority.Promotions, "Who is promoting us this month?");

            CreatePageTypeFile(PageTypeEnum.Competitions, PageTypePriority.Competitions, "Competition - Name Our Ghost");

            CreatePageTypeFile(PageTypeEnum.Partners, PageTypePriority.Partners, "Want to partner with us?");

            CreatePageTypeFile(PageTypeEnum.Partnerships, PageTypePriority.Partnerships, "Partnering with Ghost Pubs");

            CreatePageTypeFile(PageTypeEnum.FeaturedPartner, PageTypePriority.FeaturedPartner, "Who are our partners?");

            CreatePageTypeFile(PageTypeEnum.Contributors, PageTypePriority.Contributors, "Credits to our contributors");

            CreatePageTypeFile(PageTypeEnum.About, PageTypePriority.About, "About Ghost Pubs");

            CreatePageTypeFile(
                PageTypeEnum.Home,
                PageTypePriority.Home,
                "We have the largest haunted pub directory. Please make your selection below!",
                title: "Haunted pubs with a ghostly difference - Welcome to Ghost Pubs!");

            CreatePageTypeFile(PageTypeEnum.FaqBrewery,
                PageTypePriority.FaqBrewery, "FAQs that Breweries ask about Ghost Pubs");

            CreatePageTypeFile(PageTypeEnum.FaqPub, PageTypePriority.FaqPub, "FAQs that Publicans ask about Ghost Pubs");

            CreatePageTypeFile(PageTypeEnum.Newsletter,
                PageTypePriority.Newsletter, "Sign up for our newsletter here for goodies!");

            CreatePageTypeFile(PageTypeEnum.Accessibility,
                PageTypePriority.Accessibility, "What is our Accessibility Policy?");

            CreatePageTypeFile(PageTypeEnum.Terms, PageTypePriority.Terms, "Terms and conditions");

            CreatePageTypeFile(PageTypeEnum.ContactUs, PageTypePriority.ContactUs, "Contact Us");

            CreatePageTypeFile(PageTypeEnum.Privacy, PageTypePriority.Privacy, "Privacy policy");
        }

        private void GenerateLeaderboard()
        {
            List<PageLinkModel> data = null;

            if (!_isDeprecated)
            {
                data = GetLeaderboardData();

                CreatePageTypeFile(PageTypeEnum.Sitemap,
                    PageTypePriority.Sitemap, "Sitemap: Pub leaderboard of most haunted areas in UK", data);
            }
        }

        public List<PageLinkModel> GetLeaderboardData()
        {
            var results = _queryManager.GetSitemapData(_currentRoot);

            return results;
        }

        private void UpdateOrganisations(IEnumerable<Org> orgsToUpdate)
        {
            foreach (var org in orgsToUpdate)
            {
                SourceAndApplyApiData(org);

                _commandManager.Save();
            }
        }

        private void SourceAndApplyApiData(Org org)
        {
            if (org.HasTriedAllApis) return;

            var elements = _thirdPartyApiManager.ReadElements(org);

            SourceAndApplyApiData(org, elements);
        }

        private void SourceAndApplyApiData(Org org, IEnumerable<XElement> elements)
        {
            if (elements == null) return;

            foreach (var element in elements)
            {
                SourceAndApplyApiData(org, element);
            }
        }

        private void SourceAndApplyApiData(Org org, XElement element)
        {
            if (element == null) return;

            if (element.ToString().Contains("GeocodeResponse"))
            {
                SourceAndApplyGoogleMapsApiData(org, element);
            }
            else
            {
                SourceAndApplyPostcodeApiData(org, element);
            }
        }

        private void SourceAndApplyPostcodeApiData(Org org, XElement element)
        {
            var result = new XmlResult();

            if (!org.LaTried)
            {
                result = _thirdPartyApiManager.RequestLaApiResponse(element);

                org.LaData = result.Result.ToString();
            }
            else
            {
                result.ResultType = ResultTypeEnum.AlreadyTried;
            }

            if (result.ResultType != ResultTypeEnum.Success) return;

            _commandManager.UpdateOrgFromLaApiResponse(org, element);

            if (org.LaCode == null) return;

            var authority = _queryManager.GetAuthority(org.LaCode);

            if (authority == null) return;

            _commandManager.UpdateAuthority(org, authority.Id);
        }

        private void SourceAndApplyGoogleMapsApiData(Org org, XElement element)
        {
            var result = new XmlResult();

            if (!org.Tried)
            {
                result = _thirdPartyApiManager.RequestGoogleMapsApiResponse(element);

                org.GoogleMapData = result.Result.ToString();
            }
            else
            {
                result.ResultType = ResultTypeEnum.AlreadyTried;
            }

            if (result.ResultType != ResultTypeEnum.Success) return;

            _commandManager.UpdateOrgFromGoogleResponse(org, element);
        }

        private IEnumerable<Authority> CreateTopLevelRegionsFile(string currentRoot)
        {
            var results = _queryManager.GetRegions()
                .ToList();

            var viewModel = OutputViewModel.CreateAllUkRegionsOutputViewModel(currentRoot, results);

            // todo - it works but gets overwritten
            WriteFile(viewModel);

            return results;
        }

        private IEnumerable<Authority> CreateRegionFile(Authority currentRegion, string currentRegionPath,
            Int32 orgsInRegionCount)
        {
            // write region directory
            FileSystemHelper.CreateFolders(currentRegionPath, _isDeprecated);

            // region file needs knowledge of its counties
            // should be list of counties that have ghost pubs?
            // var countiesInRegion = currentRegion.Counties.ToList();

            var hauntedCountiesInRegion = _queryManager.GetHauntedCountiesInRegion(currentRegion.Id).ToList();

            var regionModel = _queryManager.PrepareRegionModel(currentRegion, currentRegionPath, orgsInRegionCount,
                hauntedCountiesInRegion, _currentRoot, _history);

            WriteFile(regionModel);

            return hauntedCountiesInRegion;
        }

        private void GenerateGeographicHtmlPages()
        {
            var regions = CreateTopLevelRegionsFile(_currentRoot);

            foreach (var currentRegion in regions)
            {
                // todo - dpc - the problem is here - there are no orgs in these regions but we know that wrong
                CreateRegionFiles(currentRegion);
            }
        }

        private void CreateRegionFiles(Authority currentRegion)
        {
            var currentRegionPath = _queryManager.BuildPath(_currentRoot, currentRegion.Name);

            CreateAllCountyFilesForRegion(currentRegion, currentRegionPath);
        }

        private void CreateAllCountyFilesForRegion(Authority currentRegion, string currentRegionPath)
        {
            var orgsInRegionCount = currentRegion.Orgs.Count(
                y => y.HauntedStatus.HasValue && y.HauntedStatus.Value);

            if (orgsInRegionCount == 0) return;

            var countiesInRegion = CreateRegionFile(currentRegion, currentRegionPath, orgsInRegionCount);

            foreach (var currentCounty in countiesInRegion)
            {
                if (currentCounty == null) continue;

                if (currentRegionPath == null) continue;

                var currentCountyPath = _queryManager.BuildPath(currentRegionPath, currentCounty.Name);

                if (currentCountyPath == null) continue;

                CreateCountyFiles(currentRegion, currentCountyPath, currentCounty.Name, currentRegionPath,
                    currentCounty.Name, currentCounty.Id);
            }
        }

        private void CreateCountyFiles(Authority currentRegion, string currentCountyPath, string currentCountyName,
            string currentRegionPath, string currentCountyDescription, int currentCountyId)
        {
            FileSystemHelper.CreateFolders(currentCountyPath, _isDeprecated);

            // write them out backwards (so alphabetical from previous) and keep towns together (so need pub has a better chance to be in the same town) 
            var orgsInCounty =
                currentRegion.Orgs.Where(org =>
                    org.Authority.Name == currentCountyName
                    && org.Town != null
                    && org.HauntedStatus.HasValue
                    && org.HauntedStatus.Value)
                    .OrderByDescending(org => org.Town)
                    .ThenByDescending(org => org.TradingName)
                    .ToList();

            var townsInCounty = orgsInCounty
                .Select(org => org.Town)
                .Distinct()
                .ToList();

            CreateCountyFile(currentCountyName, currentCountyId, currentCountyPath, townsInCounty, currentRegion,
                orgsInCounty.Count,
                currentRegionPath);

            var pubTownLinks = new List<KeyValuePair<String, PageLinkModel>>();

            CreatePubsFiles(orgsInCounty, currentCountyPath, pubTownLinks);

            CreateTownFiles(townsInCounty, currentCountyPath, pubTownLinks, currentCountyName, currentRegion,
                currentRegionPath, currentCountyDescription, currentCountyId);
        }

        private void CreatePubsFiles(IEnumerable<Org> orgsInCounty, string currentCountyPath,
            ICollection<KeyValuePair<string, PageLinkModel>> pubTownLinks)
        {
            foreach (var currentOrg in orgsInCounty)
            {
                var currentTownPath = _queryManager.BuildPath(currentCountyPath, currentOrg.Town);

                if (currentTownPath == null) continue;

                FileSystemHelper.CreateFolders(currentTownPath, _isDeprecated);

                //town file needs knowledge of its pubs, e.g., trading name
                CreatePubFile(pubTownLinks, currentTownPath, currentOrg);
            }
        }

        private void CreateTownFiles(IEnumerable<string> townsInCounty,
            string currentCountyPath, List<KeyValuePair<string, PageLinkModel>> pubTownLinks, string currentCounty,
            Authority currentRegion, string currentRegionPath, string currentCountyDescription, int currentCountyId)
        {
            // create the town pages
            foreach (var town in townsInCounty)
            {
                CreateTownFile(currentCountyPath, pubTownLinks, town, currentCounty, currentRegion,
                    currentRegionPath, currentCountyDescription, currentCountyId);
            }
        }

        private void CreateCountyFile(String currentCountyName, int currentCountyId, string currentCountyPath,
            IEnumerable<string> towns,
            Authority currentRegion, Int32 count, string currentRegionPath)
        {
            var countyModel = _queryManager.PrepareCountyModel(currentCountyName, currentCountyId, currentCountyPath,
                towns, currentRegion, count, currentRegionPath
                , _currentRoot, _history);

            // towns need to know about
            WriteFile(countyModel);
        }


        private void CreatePubFile(ICollection<KeyValuePair<string, PageLinkModel>> pubTownLinks, string currentTownPath,
            Org pub)
        {
            FileSystemHelper.CreateFolders(pub.Path, _isDeprecated);

            var pubModel = _queryManager.PreparePubModel(pubTownLinks, currentTownPath, pub, _currentRoot, _history);

            WriteFile(pubModel);
        }

        public void CreatePageTypeFile(PageTypeEnum pageType, string priority, string description,
            List<PageLinkModel> links = null, string title = null)
        {
            var path = string.Format("{0}\\{1}", _currentRoot, pageType);

            FileSystemHelper.CreateFolders(path, _isDeprecated);

            var model = _queryManager.PreparePageTypeModel(pageType, priority, description, links, title, path,
                _currentRoot);

            WriteFile(model);
        }

        private void CreateTownFile(string currentCountyPath,
            IEnumerable<KeyValuePair<string, PageLinkModel>> pubTownLinks,
            string town,
            String currentCountyName, Authority currentRegion, string currentRegionPath, string currentCountyDescription,
            int currentCountyId)
        {
            var townModel = _queryManager.PrepareTownModel(currentCountyPath, pubTownLinks, town, currentCountyName,
                currentRegion,
                currentRegionPath, currentCountyDescription, currentCountyId, _currentRoot, _history);

            WriteFile(townModel);
        }

        protected String PrepareModel(OutputViewModel data)
        {
            var output = this.PrepareView(data, data.Action.ToString());

            return output;
        }

        public void WriteFile(OutputViewModel model)
        {
            var max = Resources.MaxSize.ToInt32();

            if (!_isDeprecated)
            {
                _historySitemap.Add(model.SitemapItem);

                _history.Add(model);

                if (_history.Count > max)
                {
                    _history.RemoveAt(0);
                }

                WritePage(model);
            }
            else
            {
                WriteMissingPage(model);
            }
        }

        private void WritePage(OutputViewModel model)
        {
            var contents = PrepareModel(model);

            if (model.Unc == null) return;

            var fullFilePath = String.Concat(model.Unc.ToLower(), @"\", "detail.html");

            FileSystemHelper.WriteFile(fullFilePath, contents);
        }

        private void WriteMissingPage(OutputViewModel model)
        {
            var missing = model.DeepClone();

            missing.Action = PageTypeEnum.Missing;

            var contents = PrepareModel(missing);

            if (!contents.IsNotNullOrEmpty()) return;

            FileSystemHelper.WriteFile(String.Concat(model.Unc.ToLower(), @"\", "detail.html").RedirectionalFormat(),
                contents);
        }
    }
}