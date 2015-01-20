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

        List<Authority> GetAllAuthorities();

        List<PageLinkModel> GetSitemapData();

        // weird

        OutputViewModel PrepareAuthorityModel(Authority authority, List<PageLinkModel> localities, int count);

        OutputViewModel PrepareOrgModel(
            Org org);

        OutputViewModel PrepareLocalityModel(
            IEnumerable<KeyValuePair<string, PageLinkModel>> orgLocalityLinks, string locality,
            Authority authority);

        OutputViewModel PreparePageTypeModel(PageTypeEnum pageType, string priority, string description,
            List<PageLinkModel> links,
            string title);

        OutputViewModel PrepareRegionModel(Authority region, int orgsInRegionCount,
            IEnumerable<Authority> hauntedAuthoritiesInRegion);

        // no db dependencies
        String PrepareWebmasterSitemap(List<String> items);
    }
}