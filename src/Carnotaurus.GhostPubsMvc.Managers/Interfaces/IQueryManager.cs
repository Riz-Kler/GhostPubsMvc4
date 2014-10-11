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

        IEnumerable<Org> GetMissingInfoOrgsToUpdate();

        County GetCounty(string name);

        IEnumerable<County> GetHauntedCountiesInRegion(Int32 regionId);

        IEnumerable<Region> GetRegions();

        XElement ReadXElement(Org missingInfoOrg);

        List<PageLinkModel> GetSitemapData(String root);
    }
}