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

        IEnumerable<Authority> GetHauntedFirstDescendantAuthoritiesInRegion(Int32 regionId);

        IEnumerable<Authority> GetRegions();

        //List<PageLinkModel> GetSitemapData(String root);

        // weird

        OutputViewModel PrepareAuthorityModel(Authority authority, IEnumerable<string> towns, int count,
            string currentRoot, List<OutputViewModel> history);

        OutputViewModel PreparePubModel(
            Org pub, string currentRoot, List<OutputViewModel> history
            );

        OutputViewModel PrepareLocalityModel(
            IEnumerable<KeyValuePair<string, PageLinkModel>> pubTownLinks, string town,
            Authority currentCounty,
            string currentRoot, List<OutputViewModel> history);

        OutputViewModel PreparePageTypeModel(PageTypeEnum pageType, string priority, string description,
            List<PageLinkModel> links,
            string title, string currentRoot);

        OutputViewModel PrepareRegionModel(Authority currentRegion, int orgsInRegionCount,
            IEnumerable<Authority> hauntedCountiesInRegion, string currentRoot,
            List<OutputViewModel> history);

        // no db dependencies
        //String PrepareWebmasterSitemap(List<String> items);

        string BuildPath(Boolean isLevelled, params String[] builder);
    }
}