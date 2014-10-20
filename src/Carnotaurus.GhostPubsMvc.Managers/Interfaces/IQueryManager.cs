using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Carnotaurus.GhostPubsMvc.Data.Models.Entities;
using Carnotaurus.GhostPubsMvc.Data.Models.ViewModels;

namespace Carnotaurus.GhostPubsMvc.Managers.Interfaces
{
    public interface IQueryManager
    {
        // Todo - each of these methods should return a QueryResult class

        IEnumerable<Org> GetOrgsToUpdate();

        County GetCounty(string name);

        IEnumerable<County> GetHauntedCountiesInRegion(Int32 regionId);

        IEnumerable<Region> GetRegions();

        List<XElement> ReadElements(Org org);

        List<PageLinkModel> GetSitemapData(String root);

        // no db dependencies
        void WriteWebmasterSitemap(List<String> items, String currentRoot);

        string BuildPath(params String[] builder);

    }
}