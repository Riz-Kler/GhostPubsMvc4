using System;
using System.Xml.Linq;
using Carnotaurus.GhostPubsMvc.Data.Models.Entities;

namespace Carnotaurus.GhostPubsMvc.Managers.Interfaces
{
    public interface ICommandManager
    {
        // Todo - each of these methods should return a CommandResult class

        String CurrentUserName { get; }

        void UpdateOrgFromGoogleResponse(Org org, XContainer element);

        void UpdateOrgFromLaApiResponse(Org org, XContainer result);

        void Save();

        void UpdateAuthority(Org org, int id);
    }
}