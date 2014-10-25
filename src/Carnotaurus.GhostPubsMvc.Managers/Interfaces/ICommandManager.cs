using System;
using System.Xml.Linq;
using Carnotaurus.GhostPubsMvc.Data.Models.Entities;
using Carnotaurus.GhostPubsMvc.Managers.Implementation;

namespace Carnotaurus.GhostPubsMvc.Managers.Interfaces
{
    public interface ICommandManager
    {
        // Todo - each of these methods should return a CommandResult class

        String CurrentUserName { get; }

        void UpdateOrgFromGoogleResponse(Org org, XContainer element, CountyAdminPair countyAdmin);

        void UpdateOrgFromLaApiResponse(Org org, XContainer result);

        void Save();

        void UpdateCounty(Org org, int id);

        void UpdateCouncil(Org org, int match);
    }
}