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

        private readonly List<String> _historySitemap;
        private readonly IQueryManager _queryManager;
        private readonly IThirdPartyApiManager _thirdPartyApiManager;

        private String _currentRoot = String.Empty;
        private Guid _generationId;

        public TemplateController(IQueryManager queryManager, ICommandManager commandManager,
            IThirdPartyApiManager thirdPartyApiManager)
        {
            _commandManager = commandManager;

            _queryManager = queryManager;

            _thirdPartyApiManager = thirdPartyApiManager;

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
            var orgsToUpdate = _queryManager.GetOrgsToUpdate();

            UpdateOrganisations(orgsToUpdate);

            _currentRoot = String.Format(@"C:\Carnotaurus\{0}\haunted-pubs\",
                _generationId);

            if (_currentRoot != null)
            {
                FileSystemHelper.EnsureFolders(_currentRoot, false);
                FileSystemHelper.EnsureFolders(string.Format("{0}\\uk", _currentRoot), false);
            }

            // copy images
            CopyImageFiles();

            GenerateHtmlLeastHauntedPage();

            GenerateSimpleHtmlPages();

            GenerateHtmlHomePage();
            
            GenerateHtmlSitemap();

            GenerateContent();

            GenerateGoogleWebmasterSitemap();
        }

        private void CopyImageFiles()
        {
            const string source =
                @"C:\Carnotaurus\GhostPubsMvc4\src\Carnotaurus.GhostPubsMvc.Web\Carnotaurus.GhostPubsMvc.Web\Images";

            FileSystemHelper.CopyFolder(
                source,
                string.Format("{0}\\uk", _currentRoot)
                );
        }

        private void GenerateGoogleWebmasterSitemap()
        {
            var webmasterSitemap = _queryManager.PrepareWebmasterSitemap(_historySitemap);

            var fullFilePath = String.Format("{0}/ghostpubs-sitemap.xml", _currentRoot);

            FileSystemHelper.WriteFile(fullFilePath, webmasterSitemap);
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
                //Division = "Bridgend"

                // Sc District
                //Name = "Scotland",
                //Division = "Glasgow City"

                //// NI District
                //Name = "Northern Ireland",
                //Division = "Strabane"

                // Met County
                Name = "North East",
                Division = "Tyne and Wear"

                //Name = "North West",
                //Division = "Greater Manchester"

                //Name = "South West",
                //Division = "Devon",

                //// County
                //Name = "North West",
                //Division = "Cumbria"

                // British Isles
            };

            // GenerateGeographicHtmlPages(filter);
            GenerateGeographicHtmlPages(null);
        }

        private void GenerateSimpleHtmlPages()
        {
            CreatePageTypeFile(PageTypeEnum.Promotions, PageTypePriority.Promotions, "Who is promoting us this month?");

            CreatePageTypeFile(PageTypeEnum.Competitions, PageTypePriority.Competitions, "Competition - Name Our Ghost");

            CreatePageTypeFile(PageTypeEnum.About, PageTypePriority.About, "About Ghost Pubs");

            // Card #235 - (3) Address FAQs

            CreatePageTypeFile(PageTypeEnum.Faqs, PageTypePriority.FaqPub, "FAQs about us");

            CreatePageTypeFile(PageTypeEnum.Newsletter,
                PageTypePriority.Newsletter, "Sign up for our newsletter here for goodies!");

            CreatePageTypeFile(PageTypeEnum.Accessibility,
                PageTypePriority.Accessibility, "What is our Accessibility Policy?");

            CreatePageTypeFile(PageTypeEnum.Terms, PageTypePriority.Terms, "Terms and conditions");

            CreatePageTypeFile(PageTypeEnum.Privacy, PageTypePriority.Privacy, "Privacy policy");
        }

        private void GenerateHtmlHomePage()
        {
            var results = _queryManager.GetAllAuthorities()
                .Where(x => x.HasHauntedOrgs
                    && !x.IsCrossBorderArea
                    && !x.IsEngland);

            var links = results.Select(result => new PageLinkModel
            {
                Filename = result.CleanQualifiedName,
                Id = result.Id,
                Title = result.QualifiedName,
                Text = result.QualifiedName,
                Total = result.HauntedPubCount
            })
                .OrderBy(o => o.Text)
                .ToList();

            CreatePageTypeFile(
                PageTypeEnum.Home,
                PageTypePriority.Home,
                "We have the largest haunted pub directory. Please make your selection below!",
                links,
                "Haunted pubs with a ghostly difference - Welcome to Ghost Pubs!");
        }

        private void GenerateHtmlLeastHauntedPage()
        {
            var results = _queryManager.GetAllAuthorities()
                .Where(x => !x.HasHauntedOrgs)
                .ToList()
                .OrderBy(o => o.IsEngland).ThenBy(o => o.FullyQualifiedNameParentFirst)
                .ToList()
                ;

            var links = results.Select(result => new PageLinkModel
            {
                Filename = result.CleanQualifiedName,
                Id = result.Id,
                Title = result.FullyQualifiedName,
                Text = result.FullyQualifiedNameParentFirst,
                Total = result.HauntedPubCount
            })
                .ToList();

            CreatePageTypeFile(
                PageTypeEnum.LeastHaunted,
                PageTypePriority.Competitions,
                "Can you find a haunted pub in any of the areas below?",
                links,
                "The least haunted local authorities in the British Isles");
        }

        private void GenerateHtmlSitemap()
        {
            var data = GetSitemapData();

            CreatePageTypeFile(PageTypeEnum.Sitemap,
                PageTypePriority.Sitemap, "Sitemap: Pub leaderboard of most haunted areas in UK", data);
        }

        public List<PageLinkModel> GetSitemapData()
        {
            var results = _queryManager.GetSitemapData();

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
                ? new PageLinkModel
                {
                    Text = x.Name,
                    Title = x.Name,
                    Filename = x.CleanQualifiedName,
                    Total = x.HauntedPubCount
                }
                : null)
                .OrderBy(x => x.Text)
                .ToList();

            var metaDescription = string.Format("Haunted pubs in {0}",
                regions.Select(region => region.Name).OxfordAnd())
                .SeoMetaDescriptionTruncate();

            var articleDescription = string.Format("Haunted pubs in {0}",
                regions.Select(region => region.Name).OxfordAnd());

            regions = null;

            var viewModel = OutputViewModel.CreateAllUkRegionsOutputViewModel(currentRoot, pageLinks, metaDescription,
                articleDescription);

            WriteFile(viewModel);
        }

        private List<Authority> CreateRegionHeaderFile(Authority region,
            Int32 orgsInRegionCount)
        {
            // region file needs knowledge of its counties
            // should be list of counties that have ghost pubs?
            // var countiesInRegion = currentRegion.Counties.ToList();

            var inRegion = _queryManager.GetHauntedFirstDescendantAuthoritiesInRegion(region.Id).ToList();

            var regionModel = _queryManager.PrepareRegionModel(region, orgsInRegionCount,
                inRegion);

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
                if (filterModel == null ||
                    ((filterModel.Name.IsNullOrEmpty() || currentRegion.Name == filterModel.Name)))
                {
                    CreateAllFilesForRegion(currentRegion, filterModel);
                }
            }
        }

        private void CreateAllFilesForRegion(Authority region, RegionFilterModel filterModel)
        {
            int orgsInRegionCount;

            if (region.IsOutsideUnitedKingdom)
            {
                orgsInRegionCount = region.HauntedPubCount;
            }
            else
            {
                var firstDescendantAuthoritiesInRegion =
                    _queryManager.GetHauntedFirstDescendantAuthoritiesInRegion(region.Id);

                orgsInRegionCount = firstDescendantAuthoritiesInRegion.Sum(x => x.HauntedPubCount);
            }

            if (orgsInRegionCount == 0) return;

            var inRegion = CreateRegionHeaderFile(region, orgsInRegionCount);

            if (!inRegion.Any())
            {
                CreateAuthorityFilesTop(region);
            }
            else
            {
                // todo - work out why filterModel == null?
                var filtered = inRegion.Where(authority => filterModel == null || filterModel.Division.IsNullOrEmpty() ||
                                                    authority.Name == filterModel.Division);

                foreach (var authority in filtered)
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
                    authority.HauntedOrgs.Where(org =>
                        org.Authority.Name == authority.Name)
                        .OrderByDescending(org => org.Locality)
                        .ThenByDescending(org => org.TradingName);

                var localities = orgs
                    .Select(p => p.Locality)
                    .Distinct()
                    .OrderBy(o => o)
                    .ToList();

                CreateAuthorityFile(authority, localities,
                    orgs.Count()
                    );

                var links = CreateOrgFiles(orgs);

                CreateLocalityFiles(localities, links, authority);
            }
            else
            {
                var districts = authority.Authoritys
                    .Select(a => a.QualifiedName)
                    .Distinct()
                    .ToList();

                CreateAuthorityFile(authority, districts,
                    authority.HauntedPubCount
                    );
            }
        }

        private List<KeyValuePair<String, PageLinkModel>> CreateOrgFiles(IEnumerable<Org> orgs)
        {
            var localityLinks = new List<KeyValuePair<String, PageLinkModel>>();

            foreach (var org in orgs)
            {
                //town file needs knowledge of its pubs, e.g., trading name
                CreateOrgFile(org);

                localityLinks.Add(new KeyValuePair<string, PageLinkModel>(org.Locality, org.ExtractLink()));
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

            var links = locations.Select(locality => new PageLinkModel
            {
                Text = locality,
                Title = locality,
                Filename = authority.IsCounty
                    ? locality.Clean()
                    : locality.In(authority.CleanQualifiedName, true),
                Total = authority.IsCounty
                    ? authority.Authoritys
                        .First(a => a.QualifiedName == locality).HauntedOrgs
                        .Count()
                    : authority.HauntedOrgs
                        .Count(x => x.Locality == locality)
            }).ToList();

            var model = _queryManager.PrepareAuthorityModel(authority,
                links, count);

            WriteFile(model);
        }

        private void CreateOrgFile(Org org)
        {
            if (org == null) throw new ArgumentNullException("org");

            var model = _queryManager.PrepareOrgModel(org);

            WriteFile(model);
        }

        public void CreatePageTypeFile(PageTypeEnum pageType, string priority, string description,
            List<PageLinkModel> links = null, string title = null)
        {
            if (links == null)
            {
                links = new List<PageLinkModel>();
            }

            var model = _queryManager.PreparePageTypeModel(pageType, priority, description, links, title);

            var pathOverride = string.Format("{0}{1}", _currentRoot, "uk\\");

            FileSystemHelper.CreateFolders(pathOverride, false);

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
                authority
                );

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

            if (_historySitemap != null) _historySitemap.Add(model.SitemapItem);

            WritePage(model, pathOverride);
        }

        private void WritePage(OutputViewModel model, string pathOverride)
        {
            if (model == null) throw new ArgumentNullException("model");

            if (model.Filename == null) return;

            var toUse = !pathOverride.IsNullOrEmpty()
                ? pathOverride
                : _currentRoot.ToLower();

            if (toUse == null)
            {
                throw new ArgumentNullException("toUse");
            }

            var fullFilePath = String.Format("{0}{1}{2}",
                toUse,
                model.FriendlyFilename,
                ".html");

            if (model.FriendlyFilename == null) return;

            var contents = PrepareModel(model);

            if (contents == null) throw new ArgumentNullException("contents");

            model = null;

            FileSystemHelper.WriteFile(fullFilePath, contents);
        }
    }
}