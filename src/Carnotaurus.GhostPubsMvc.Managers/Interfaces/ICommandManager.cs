using System;
using System.Xml.Linq;
using Carnotaurus.GhostPubsMvc.Common.Bespoke.Enumerations;
using Carnotaurus.GhostPubsMvc.Data.Models.Entities;

namespace Carnotaurus.GhostPubsMvc.Managers.Interfaces
{
    public interface ICommandManager
    {
        // Todo - each of these methods should return a CommandResult class

        String CurrentUserName { get; }

        string UpdateAdministrativeAreaLevels(XContainer container, Org org);

        ResultTypeEnum UpdateOrganisationByGoogleMapsApi(Org org, XElement element);

        ResultTypeEnum UpdateOrganisationByLaApi(Org org, XElement element);

        void Save();

        void UpdateCounty(Org org, County match);
         
        void UpdateCouncil(Org org, int match);
         
    }
}