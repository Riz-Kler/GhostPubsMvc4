﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;
using Carnotaurus.GhostPubsMvc.Common.Bespoke;
using Carnotaurus.GhostPubsMvc.Common.Bespoke.Enumerations;
using Carnotaurus.GhostPubsMvc.Common.Extensions;
using Carnotaurus.GhostPubsMvc.Common.Helpers;
using Carnotaurus.GhostPubsMvc.Data.Models;
using Carnotaurus.GhostPubsMvc.Data.Models.Entities;
using Carnotaurus.GhostPubsMvc.Data.Models.ViewModels;
using Carnotaurus.GhostPubsMvc.Managers.Interfaces;

namespace Carnotaurus.GhostPubsMvc.Controllers
{
    public class TemplateController : Controller
    {
        private readonly ICommandManager _commandManager;

        // todo - come back sitemap history
        // private readonly List<String> _historySitemap;
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

            // _historySitemap = new List<String>();
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

            _currentRoot = String.Format(@"C:\Carnotaurus\{0}\haunted-pubs\",
                _generationId.ToString().Dashify());

            if (_currentRoot != null)
            {
                FileSystemHelper.EnsureFolders(_currentRoot, _isDeprecated);
            }

            GenerateSimpleHtmlPages();

            GenerateContent();


            // todo - come back sitemap history
            // GenerateLeaderboard();


            // todo - come back sitemap history
            // GenerateWebmasterSitemap();
        }

        // todo - come back sitemap history
        //private void GenerateWebmasterSitemap()
        //{
        //    var webmasterSitemap = _queryManager.PrepareWebmasterSitemap(_historySitemap);

        //    var fullFilePath = String.Format("{0}/ghostpubs-sitemap.xml", _currentRoot);

        //    FileSystemHelper.WriteFile(fullFilePath, webmasterSitemap);
        //}

        private void GenerateDeadContent()
        {
            _isDeprecated = true;

            GenerateContent();
        }

        private void GenerateContent()
        {
            var filter = new RegionFilterModel
            {
                // UA
                //Name = "North West",
                //Division = "Cheshire West and Chester"

                // London borough
                //Name = "London",
                //Division = "Bromley"

                // W District
                //Name = "Wales",
                //Division = "The Vale of Glamorgan"

                // Sc District
                //Name = "Scotland",
                //Division = "Glasgow City"

                //// NI District
                //Name = "Northern Ireland",
                //Division = "Strabane"

                // Met County
                //Name = "North East",
                //Division = "Tyne and Wear"

                Name = "South West",
                Division = "Devon",


                //// County
                //Name = "North West",
                //Division = "Cumbria"
            };

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
            if (_isDeprecated) return;

            var data = GetLeaderboardData();

            CreatePageTypeFile(PageTypeEnum.Sitemap,
                PageTypePriority.Sitemap, "Sitemap: Pub leaderboard of most haunted areas in UK", data);
        }

        public List<PageLinkModel> GetLeaderboardData()
        {
            var results = _queryManager.GetSitemapData(_currentRoot);

            return results;
        }

        private void UpdateOrganisations(IEnumerable<Org> orgsToUpdate)
        {
            if (orgsToUpdate == null) throw new ArgumentNullException("orgsToUpdate");

            foreach (var org in orgsToUpdate)
            {
                SourceAndApplyApiData(org);

                _commandManager.Save();
            }
        }

        private void SourceAndApplyApiData(Org org)
        {
            if (org == null) throw new ArgumentNullException("org");

            if (org.HasTriedAllApis) return;

            var elements = _thirdPartyApiManager.ReadElements(org);

            SourceAndApplyApiData(org, elements);
        }

        private void SourceAndApplyApiData(Org org, IEnumerable<XElement> elements)
        {
            if (org == null) throw new ArgumentNullException("org");
            if (elements == null) return;

            foreach (var element in elements)
            {
                SourceAndApplyApiData(org, element);
            }
        }

        private void SourceAndApplyApiData(Org org, XElement element)
        {
            if (org == null) throw new ArgumentNullException("org");
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
            if (org == null) throw new ArgumentNullException("org");
            if (element == null) throw new ArgumentNullException("element");

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
            if (org == null) throw new ArgumentNullException("org");
            if (element == null) throw new ArgumentNullException("element");

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

        private void CreateTopLevelRegionsFile(string currentRoot)
        {
            var regions = _queryManager.GetRegions()
                .ToList();

            var pageLinks = regions.Select(x => x.Name != null
                ? new PageLinkModel(currentRoot)
                {
                    Text = x.Name,
                    Title = x.Name,
                    Filename = x.QualifiedName.Dashify()
                }
                : null).OrderBy(x => x.Text).ToList();

            var metaDescription = string.Format("Haunted pubs in {0}",
                    regions.Select(region => region.Name).OxfordAnd())
                    .SeoMetaDescriptionTruncate();

            var articleDescription = string.Format("Haunted pubs in {0}",
                regions.Select(region => region.Name).OxfordAnd());

            regions = null;

            var viewModel = OutputViewModel.CreateAllUkRegionsOutputViewModel(currentRoot, pageLinks, metaDescription, articleDescription);

            WriteFile(viewModel);
        }

        private IEnumerable<Authority> CreateRegionHeaderFile(Authority region,
            Int32 orgsInRegionCount)
        {
            // region file needs knowledge of its counties
            // should be list of counties that have ghost pubs?
            // var countiesInRegion = currentRegion.Counties.ToList();

            var inRegion = _queryManager.GetHauntedFirstDescendantAuthoritiesInRegion(region.Id).ToList();

            var regionModel = _queryManager.PrepareRegionModel(region, orgsInRegionCount,
                inRegion, _currentRoot);

            WriteFile(regionModel);

            return inRegion;
        }

        private void GenerateGeographicHtmlPages()
        {
            GenerateGeographicHtmlPages(null);
        }

        private void GenerateGeographicHtmlPages(RegionFilterModel filterModel)
        {
            CreateTopLevelRegionsFile(_currentRoot);

            var regions = _queryManager.GetRegions()
                .ToList();

            foreach (var currentRegion in regions)
            {
                if (filterModel == null || ((filterModel.Name.IsNullOrEmpty() || currentRegion.Name == filterModel.Name)))
                {
                    CreateAllFilesForRegion(currentRegion, filterModel);
                }
            }
        }

        private void CreateAllFilesForRegion(Authority region, RegionFilterModel filterModel)
        {
            var firstDescendantAuthoritiesInRegion =
                _queryManager.GetHauntedFirstDescendantAuthoritiesInRegion(region.Id);

            var orgsInRegionCount = firstDescendantAuthoritiesInRegion.Sum(x => x.CountHauntedOrgs);

            if (orgsInRegionCount == 0) return;

            var inRegion = CreateRegionHeaderFile(region, orgsInRegionCount);

            foreach (var authority in inRegion)
            {
                if (filterModel == null || filterModel.Division.IsNullOrEmpty() || authority.Name == filterModel.Division)
                {
                    CreateAuthorityFilesTop(authority);
                }
            }
        }

        private void CreateAuthorityFilesTop(Authority authority)
        {
            if (authority == null) return;

            if (authority.Authoritys != null && authority.Authoritys.Any())
            {
                if (authority.Authoritys == null) return;

                foreach (var district in authority.Authoritys)
                {
                    CreateAuthorityFiles(district);
                }
            }

            CreateAuthorityFiles(authority);
        }

        private void CreateAuthorityFiles(Authority authority)
        {
            if (authority == null) throw new ArgumentNullException("authority");

            if (!authority.IsCounty)
            {
                var orgs =
                    authority.Orgs.Where(org =>
                        org.Authority.Name == authority.Name
                        && org.Locality != null
                        && org.HauntedStatus.HasValue
                        && org.HauntedStatus.Value
                        && org.AddressTypeId == 1)
                        .OrderByDescending(org => org.Locality)
                        .ThenByDescending(org => org.TradingName);
                       // .ToList();

                var localities = orgs
                    .Select(p => p.Locality)
                    .Distinct()
                    .ToList();

                CreateAuthorityFile(authority, localities,
                    orgs.Count()
                    );

                var links = CreatePubsFiles(orgs);

                CreateLocalityFiles(localities, links, authority);
            }
            else
            {
                var districts = authority.Authoritys
                    .Select(a => a.QualifiedName)
                    .Distinct()
                    .ToList();

                CreateAuthorityFile(authority, districts,
                    authority.CountHauntedOrgs
                    );
            }
        }

        private List<KeyValuePair<String, PageLinkModel>> CreatePubsFiles(IEnumerable<Org> orgs)
        {
            var localityLinks = new List<KeyValuePair<String, PageLinkModel>>();

            foreach (var org in orgs)
            {
                //town file needs knowledge of its pubs, e.g., trading name
                CreateOrgFile(org);

                localityLinks.Add(new KeyValuePair<string, PageLinkModel>(org.Locality, org.ExtractLink(_currentRoot)));
            }

            return localityLinks;
        }

        private void CreateLocalityFiles(IEnumerable<string> localities,
            List<KeyValuePair<string, PageLinkModel>> orgLocalityLinks, Authority authority
            )
        {
            if (localities == null) throw new ArgumentNullException("localities");
            if (orgLocalityLinks == null) throw new ArgumentNullException("orgLocalityLinks");
            if (authority == null) throw new ArgumentNullException("authority");

            // create the locality pages
            foreach (var locality in localities)
            {
                CreateLocalityFile(orgLocalityLinks, locality, authority);
            }
        }

        private void CreateAuthorityFile(Authority authority,
            IEnumerable<string> locations,
            Int32 count)
        {
            if (authority == null) throw new ArgumentNullException("authority");
            if (locations == null) throw new ArgumentNullException("locations");
             
            var links = locations.Select(locality => new PageLinkModel(_currentRoot)
            {
                Text = locality,
                Title = locality,
                Filename = authority.IsCounty
                    ? locality
                    : locality.InDashifed(authority.QualifiedName)
            }).ToList();
             
            var model = _queryManager.PrepareAuthorityModel(authority,
                links, count, _currentRoot);

            WriteFile(model);
        }


        private void CreateOrgFile(Org org)
        {
            if (org == null) throw new ArgumentNullException("org");

            var model = _queryManager.PrepareOrgModel(org, _currentRoot);

            WriteFile(model);
        }

        public void CreatePageTypeFile(PageTypeEnum pageType, string priority, string description,
            List<PageLinkModel> links = null, string title = null)
        {
            if (links == null)
            {
                links = new List<PageLinkModel>();
            }

            var model = _queryManager.PreparePageTypeModel(pageType, priority, description, links, title,
                _currentRoot);

            var pathOverride = string.Format("{0}{1}", _currentRoot, "uk\\");

            FileSystemHelper.CreateFolders(pathOverride, _isDeprecated);

            WriteFile(model, pathOverride);
        }

        private void CreateLocalityFile(
            IEnumerable<KeyValuePair<string, PageLinkModel>> orgLocalityLinks,
            string locality,
            Authority authority)
        {
            if (orgLocalityLinks == null) throw new ArgumentNullException("orgLocalityLinks");
            if (locality == null) throw new ArgumentNullException("locality");
            if (authority == null) throw new ArgumentNullException("authority");

            var model = _queryManager.PrepareLocalityModel(orgLocalityLinks, locality,
                authority,
                _currentRoot);

            WriteFile(model);
        }

        protected String PrepareModel(OutputViewModel data)
        {
            if (data == null) throw new ArgumentNullException("data");

            var output = this.PrepareView(data, data.Action.ToString());

            return output;
        }

        public void WriteFile(OutputViewModel model)
        {
            if (model == null) throw new ArgumentNullException("model");
            WriteFile(model, null);
        }

        public void WriteFile(OutputViewModel model, string pathOverride)
        {
            if (model == null) throw new ArgumentNullException("model");

            if (_isDeprecated) return;

            if (!pathOverride.IsNullOrEmpty())
            {
                model.CurrentRoot = pathOverride;
            }


            // todo - come back sitemap history
            // if (_historySitemap != null) _historySitemap.Add(model.SitemapItem);

            WritePage(model, pathOverride);
        }

        private void WritePage(OutputViewModel model, string pathOverride)
        {
            if (model == null) throw new ArgumentNullException("model");

            var contents = PrepareModel(model);

            if (contents == null) throw new ArgumentNullException("contents");

            if (model.Filename == null) return;

            var toUse = !pathOverride.IsNullOrEmpty()
                ? pathOverride
                : model.CurrentRoot.ToLower();

            if (toUse == null) throw new ArgumentNullException("toUse");

            if (model.FriendlyFilename == null) return;

            var fullFilePath = String.Format("{0}{1}{2}",
                toUse,
                model.FriendlyFilename,
                ".html");

            FileSystemHelper.WriteFile(fullFilePath, contents);
        }

        private void WriteMissingPage(OutputViewModel model)
        {
            if (model == null) throw new ArgumentNullException("model");
            var missing = model.DeepClone();

            missing.Action = PageTypeEnum.Missing;

            var contents = PrepareModel(missing);

            if (!contents.IsNotNullOrEmpty()) return;

            FileSystemHelper.WriteFile(String.Concat(model.Filename.ToLower(), @"\", "detail.html").RedirectionalFormat(),
                contents);
        }
    }
}