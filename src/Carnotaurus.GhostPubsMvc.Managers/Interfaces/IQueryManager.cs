using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Carnotaurus.GhostPubsMvc.Data.Models;

namespace Carnotaurus.GhostPubsMvc.Managers.Interfaces
{
    public interface IQueryManager
    {
        IEnumerable<Org> GetMissingInfoOrgsToUpdate();

        County GetCounty(string name);

        IEnumerable<County> GetHauntedCountiesInRegion(Int32 regionId);

        IEnumerable<Region> GetRegions();


        XElement ReadXElement(Org missingInfoOrg);

        // Read commands

        //List<Role> GetAllRoles();

        //List<UserProfile> GetAllUserProfiles();

        //Int32 GetTotalUserProfiles(UserSearchListInputModel model);

        //UserProfile FindUserByClientRefAndClientBranch(Int32 clientRef, Int32 clientBranch);

        //UserProfile FindUser(Int32 id);

        //UserProfile FindUser(String username);

        //UserProfile FindUserByClientRefAndClientBranch(Int32 clientRef, ClientBranchEnum clientBranch);

        //Role FindRole(String rolename);

        //PaymentInputModel GetPaymentDefaults(PaymentInputModel model, String username);

        //List<UserProfile> GetPagedUserProfiles(UserSearchListInputModel model);

        //List<PaymentCapture> GetPagedPayments(PaymentSearchListInputModel model);

        //Int32 GetTotalPayments(PaymentSearchListInputModel model);

        //PaymentCapture FindLatestPaymentByCartRef(CartRefModel model);

        //PaymentCapture GetLatestPaymentCapture(String username, CartRefModel model);
    }
}