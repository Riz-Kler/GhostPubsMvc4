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

        List<PageLinkModel> GetSitemapData(String root);

        // weird

        OutputViewModel PrepareAuthorityModel(Authority authority, IEnumerable<string> localities, int count,
            string currentRoot);

        OutputViewModel PrepareOrgModel(
            Org org, string currentRoot
            );

        OutputViewModel PrepareLocalityModel(
            IEnumerable<KeyValuePair<string, PageLinkModel>> orgLocalityLinks, string locality,
            Authority authority,
            string currentRoot);

        OutputViewModel PreparePageTypeModel(PageTypeEnum pageType, string priority, string description,
            List<PageLinkModel> links,
            string title, string currentRoot);

        OutputViewModel PrepareRegionModel(Authority region, int orgsInRegionCount,
            IEnumerable<Authority> hauntedAuthoritiesInRegion, string currentRoot);

        // no db dependencies
        String PrepareWebmasterSitemap(List<String> items);

    }
}