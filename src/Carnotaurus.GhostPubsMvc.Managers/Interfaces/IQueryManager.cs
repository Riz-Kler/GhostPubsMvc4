using System;
using System.Collections.Generic;
using Carnotaurus.GhostPubsMvc.Common.Bespoke.Enumerations;
using Carnotaurus.GhostPubsMvc.Data.Models.Entities;
using Carnotaurus.GhostPubsMvc.Data.Models.ViewModels;

namespace Carnotaurus.GhostPubsMvc.Managers.Interfaces
{
    public interface IQueryManager
    {
        // Todo - each of these methods should return a QueryResult class

        IEnumerable<Org> GetOrgsToUpdate();

        Authority GetAuthority(string code);

        IEnumerable<County> GetHauntedCountiesInRegion(Int32 regionId);

        IEnumerable<Region> GetRegions();

        List<PageLinkModel> GetSitemapData(String root);

        // weird

        OutputViewModel PrepareCountyModel(string currentCountyName, int currentCountyId,
            string currentCountyPath,
            IEnumerable<string> towns, Region currentRegion, int count, string currentRegionPath, string currentRoot,
            List<OutputViewModel> history);

        OutputViewModel PreparePubModel(ICollection<KeyValuePair<string, PageLinkModel>> pubTownLinks,
            string currentTownPath, Org pub, string currentRoot, List<OutputViewModel> history
            );

        OutputViewModel PrepareTownModel(string currentCountyPath,
            IEnumerable<KeyValuePair<string, PageLinkModel>> pubTownLinks, string town,
            string currentCountyName, Region currentRegion, string currentRegionPath, string currentCountyDescription,
            int currentCountyId, string currentRoot, List<OutputViewModel> history);

        OutputViewModel PreparePageTypeModel(PageTypeEnum pageType, string priority, string description,
            List<PageLinkModel> links,
            string title, string path, string currentRoot);

        OutputViewModel PrepareRegionModel(Region currentRegion, string currentRegionPath, int orgsInRegionCount,
            IEnumerable<County> hauntedCountiesInRegion, string currentRoot,
            List<OutputViewModel> history);

        // no db dependencies
        String PrepareWebmasterSitemap(List<String> items);

        string BuildPath(params String[] builder);
    }
}