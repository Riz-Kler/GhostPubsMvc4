using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Carnotaurus.GhostPubsMvc.Common.Bespoke;
using Carnotaurus.GhostPubsMvc.Data.Models;

namespace Carnotaurus.GhostPubsMvc.Managers.Interfaces
{
    public interface ICommandManager
    {
        // Todo - each of these methods should return a CommandResult class

        String CurrentUserName { get; }

        string UpdateAdministrativeAreaLevels(XContainer result, Org org);

        ResultTypeEnum UpdateOrganisation(Org missingInfoOrg, XElement xElement);

        void Save();

        void UpdateCounty(Org org, County match);

    }
}