using System;
using System.Collections.Generic;
using Carnotaurus.GhostPubsMvc.Common.Result;
using Carnotaurus.GhostPubsMvc.Data.Models;

namespace Carnotaurus.GhostPubsMvc.Managers.Interfaces
{
    public interface ICommandManager
    {
        String CurrentUserName { get; }

        void UpdateOrgs(IEnumerable<Org> missingInfoOrgs);

        //CommandResult SavePayment(PaymentCapture command);

        //CommandResult ModifyPayment(PaymentCapture command);

        //CommandResult ArchiveUser(UserProfile command);

        //CommandResult AddRoleToUser(UserProfile user, Role role);

        //CommandResult RemoveUserFromRole(UserProfile user, Role role);

        //CommandResult AddUser(UserProfile command);

        //CommandResult ModifyUser(UserProfile command);

        //CommandResult DeleteUser(UserProfile command);

        //CommandResult SavePayment(PaymentInputModel model);

        //CommandResult Decline(UserProfile user, Role role, String reason);

        //CommandResult Suspend(UserProfile user, Role role, String reason);

        //CommandResult Clear(UserProfile user, Role role);

        //CommandResult AddUser(CreateUserProfileInputModel model);

        //CommandResult ModifyUser(EditUserProfileInputModel model);

        //CommandResult Approve(UserProfile userProfile, Role role, String url);

        //CommandResult NotifyRegistration(RegistrationInputModel model, String url);

        //CommandResult NotifyLostPassword(LostPasswordModel model, String url, String token, UserProfile user);

        //CommandResult SavePaymentReturn(PaymentCapture model, Boolean isSuccess);
    }
}