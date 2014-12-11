using System;
using System.Collections.Generic;
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

            _currentRoot = String.Format(@"C:\Carnotaurus\{0}\haunted-pubs\",
                _generationId.ToString().Dashify());

            if (_currentRoot != null)
            {
                FileSystemHelper.EnsureFolders(_currentRoot, _isDeprecated);
            }

            // GenerateSimpleHtmlPages();

            GenerateContent();

            // GenerateLeaderboard();

            //GenerateWebmasterSitemap();
        }

        //private void GenerateWebmasterSitemap()
        //{
        //    var sm = _queryManager.PrepareWebmasterSitemap(_historySitemap);

        //    var fullFilePath = String.Format("{0}/ghostpubs-sitemap.xml", _currentRoot);

        //    FileSystemHelper.WriteFile(fullFilePath, sm);
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
                //RegionName = "North West",
                //CountyName = "Cheshire West and Chester"

                // London borough
                // RegionName = "London",
                // CountyName = "Bromley"

                // W District
                //RegionName = "Wales",
                //CountyName = "The Vale of Glamorgan"

                // Sc District
                //Name = "Scotland",
                //Division = "Glasgow City"

                //// NI District
                //Name = "Northern Ireland",
                //Division = "Strabane"

                // Met County
                Name = "North East",
                Division = "Tyne and Wear"

                //// County
                //RegionName = "North West",
                //CountyName = "Cumbria"
            };

            GenerateGeographicHtmlPages(filter);
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

        //private void GenerateLeaderboard()
        //{
        //    List<PageLinkModel> data = null;

        //    if (!_isDeprecated)
        //    {
        //        data = GetLeaderboardData();

        //        CreatePageTypeFile(PageTypeEnum.Sitemap,
        //            PageTypePriority.Sitemap, "Sitemap: Pub leaderboard of most haunted areas in UK", data);
        //    }
        //}

        //public List<PageLinkModel> GetLeaderboardData()
        //{
        //    var results = _queryManager.GetSitemapData(_currentRoot);

        //    return results;
        //}

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

            WriteFile(viewModel);

            return results;
        }

        private IEnumerable<Authority> CreateRegionHeaderFile(Authority currentRegion,
            Int32 orgsInRegionCount)
        {
            // region file needs knowledge of its counties
            // should be list of counties that have ghost pubs?
            // var countiesInRegion = currentRegion.Counties.ToList();

            var inRegion = _queryManager.GetHauntedFirstDescendantAuthoritiesInRegion(currentRegion.Id).ToList();

            var regionModel = _queryManager.PrepareRegionModel(currentRegion, orgsInRegionCount,
                inRegion, _currentRoot, _history);

            WriteFile(regionModel);

            return inRegion;
        }

        private void GenerateGeographicHtmlPages(RegionFilterModel filterModel)
        {
            var regions = CreateTopLevelRegionsFile(_currentRoot);

            foreach (var currentRegion in regions)
            {
                if (currentRegion.Name == filterModel.Name)
                {
                    // todo - dpc - the problem is here - there are no orgs in these regions but we know that wrong
                    CreateAllFilesForRegion(currentRegion, filterModel);
                }
            }
        }

        private void CreateAllFilesForRegion(Authority currentRegion, RegionFilterModel filterModel)
        {
            var firstDescendantAuthoritiesInRegion =
                _queryManager.GetHauntedFirstDescendantAuthoritiesInRegion(currentRegion.Id);

            var orgsInRegionCount = firstDescendantAuthoritiesInRegion.Sum(x => x.CountHauntedOrgs);

            if (orgsInRegionCount == 0) return;

            var inRegion = CreateRegionHeaderFile(currentRegion, orgsInRegionCount);

            foreach (var authority in inRegion)
            {
                if (authority.Name == filterModel.Division)
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
            if (!authority.IsCounty)
            {
                var pubs =
                    authority.Orgs.Where(org =>
                        org.Authority.Name == authority.Name
                        && org.Locality != null
                        && org.HauntedStatus.HasValue
                        && org.HauntedStatus.Value
                        && org.AddressTypeId == 1)
                        .OrderByDescending(org => org.Locality)
                        .ThenByDescending(org => org.TradingName)
                        .ToList();

                var localities = pubs
                    .Select(p => p.Locality)
                    .Distinct()
                    .ToList();

                CreateAuthorityFile(authority, localities,
                    pubs.Count
                    );

                var links = CreatePubsFiles(pubs);

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

        private List<KeyValuePair<String, PageLinkModel>> CreatePubsFiles(IEnumerable<Org> pubs)
        {
            var localityLinks = new List<KeyValuePair<String, PageLinkModel>>();

            foreach (var pub in pubs)
            {
                //town file needs knowledge of its pubs, e.g., trading name
                CreatePubFile(pub);

                localityLinks.Add(new KeyValuePair<string, PageLinkModel>(pub.Locality, pub.ExtractLink(_currentRoot)));
            }

            return localityLinks;
        }

        private void CreateLocalityFiles(IEnumerable<string> townsInCounty,
            List<KeyValuePair<string, PageLinkModel>> pubTownLinks, Authority currentAuthority
            )
        {
            // create the town pages
            foreach (var town in townsInCounty)
            {
                CreateTownFile(pubTownLinks, town, currentAuthority);
            }
        }

        private void CreateAuthorityFile(Authority authority,
            IEnumerable<string> locations,
            Int32 count)
        {
            var model = _queryManager.PrepareAuthorityModel(authority,
                locations, count, _currentRoot, _history);

            WriteFile(model);
        }


        private void CreatePubFile(Org pub)
        {
            var pubModel = _queryManager.PreparePubModel(pub, _currentRoot, _history);

            WriteFile(pubModel);
        }

        public void CreatePageTypeFile(PageTypeEnum pageType, string priority, string description,
            List<PageLinkModel> links = null, string title = null)
        {
            var model = _queryManager.PreparePageTypeModel(pageType, priority, description, links, title,
                _currentRoot);

            var pathOverride = string.Format("{0}{1}", _currentRoot, "uk\\");

            FileSystemHelper.CreateFolders(pathOverride, _isDeprecated);

            WriteFile(model, pathOverride);
        }

        private void CreateTownFile(
            IEnumerable<KeyValuePair<string, PageLinkModel>> pubTownLinks,
            string town,
            Authority currentAuthority)
        {
            var townModel = _queryManager.PrepareLocalityModel(pubTownLinks, town,
                currentAuthority,
                _currentRoot, _history);

            WriteFile(townModel);
        }

        protected String PrepareModel(OutputViewModel data)
        {
            var output = this.PrepareView(data, data.Action.ToString());

            return output;
        }

        public void WriteFile(OutputViewModel model)
        {
            WriteFile(model, null);
        }

        public void WriteFile(OutputViewModel model, string pathOverride)
        {
            if (model == null) throw new ArgumentNullException("model");

            var max = Resources.MaxSize.ToInt32();

            if (!_isDeprecated)
            {
                _historySitemap.Add(model.SitemapItem);

                _history.Add(model);

                if (_history.Count > max)
                {
                    _history.RemoveAt(0);
                }

                WritePage(model, pathOverride);
            }
        }

        private void WritePage(OutputViewModel model, string pathOverride)
        {
            var contents = PrepareModel(model);

            if (model.Filename == null) return;

            var toUse = !pathOverride.IsNullOrEmpty() ? pathOverride : model.CurrentRoot.ToLower();

            if (toUse == null) throw new ArgumentNullException("model");

            var fullFilePath = String.Concat(toUse, model.FriendlyFilename, ".html");

            FileSystemHelper.WriteFile(fullFilePath, contents);
        }

        //private void WriteMissingPage(OutputViewModel model)
        //{
        //    var missing = model.DeepClone();

        //    missing.Action = PageTypeEnum.Missing;

        //    var contents = PrepareModel(missing);

        //    if (!contents.IsNotNullOrEmpty()) return;

        //    FileSystemHelper.WriteFile(String.Concat(model.Filename.ToLower(), @"\", "detail.html").RedirectionalFormat(),
        //        contents);
        //}
    }
}